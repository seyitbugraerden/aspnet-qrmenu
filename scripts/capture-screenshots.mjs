// Requires Node.js 22+ and Chrome running with --remote-debugging-port=9223.
import { mkdir, writeFile } from 'node:fs/promises';

const baseUrl = process.env.MENU_URL || 'http://127.0.0.1:5069';
const debugUrl = process.env.CHROME_DEBUG_URL || 'http://127.0.0.1:9223';
const output = new URL('../docs/screenshots/', import.meta.url);
const categoriesResponse = await fetch(`${baseUrl}/api/category`);
if (!categoriesResponse.ok) throw new Error(`Category API: ${categoriesResponse.status}`);
const categories = await categoriesResponse.json();
const target = await (await fetch(`${debugUrl}/json/new?about:blank`, { method: 'PUT' })).json();
const socket = new WebSocket(target.webSocketDebuggerUrl);
await new Promise((resolve, reject) => {
    socket.addEventListener('open', resolve, { once: true });
    socket.addEventListener('error', reject, { once: true });
});
let id = 0;
const pending = new Map();
socket.addEventListener('message', event => {
    const message = JSON.parse(event.data);
    const request = pending.get(message.id);
    if (!request) return;
    clearTimeout(request.timeout);
    pending.delete(message.id);
    if (message.error) request.reject(new Error(JSON.stringify(message.error)));
    else request.resolve(message.result);
});
function command(method, params = {}) {
    return new Promise((resolve, reject) => {
        const requestId = ++id;
        const timeout = setTimeout(() => {
            pending.delete(requestId);
            reject(new Error(`${method} timed out`));
        }, 30000);
        pending.set(requestId, { resolve, reject, timeout });
        socket.send(JSON.stringify({ id: requestId, method, params }));
    });
}
async function evaluate(expression) {
    const result = await command('Runtime.evaluate', { expression, awaitPromise: true, returnByValue: true });
    if (result.exceptionDetails) throw new Error(JSON.stringify(result.exceptionDetails));
    return result.result.value;
}
const report = [];
try {
    await mkdir(output, { recursive: true });
    await command('Page.enable');
    await command('Runtime.enable');
    await command('Emulation.setEmulatedMedia', { features: [{ name: 'prefers-reduced-motion', value: 'reduce' }] });
    const pages = [
        { path: '/', name: 'ana-sayfa', width: 1440, height: 1000 },
        ...categories.map(c => ({ path: `/${c.slug}`, name: c.slug, width: 1440, height: 1000 })),
        { path: '/', name: 'ana-sayfa-mobil', width: 390, height: 844 }
    ];
    for (const page of pages) {
        await command('Emulation.setDeviceMetricsOverride', {
            width: page.width, height: page.height, deviceScaleFactor: 1, mobile: page.width < 600
        });
        const navigation = await command('Page.navigate', { url: `${baseUrl}${page.path}` });
        if (navigation.errorText) throw new Error(navigation.errorText);
        // Wait for this navigation, rather than inspecting the previous document.
        await evaluate(`new Promise((resolve, reject) => {
            const started = Date.now();
            const check = () => {
                if (location.pathname === ${JSON.stringify(page.path)} && document.readyState === 'complete') resolve();
                else if (Date.now() - started > 20000) reject(new Error('Document load timed out'));
                else setTimeout(check, 50);
            }; check();
        })`);
        const state = await evaluate(`(async () => {
            await document.fonts.ready;
            for (const img of document.images) img.loading = 'eager';
            await Promise.all([...document.images].map(img => img.decode().catch(() => {})));
            await new Promise(resolve => requestAnimationFrame(() => requestAnimationFrame(resolve)));
            return {
                title: document.title,
                images: [...document.images].map(img => ({ src: img.getAttribute('src'), loaded: img.complete && img.naturalWidth > 0 })),
                horizontalOverflow: document.documentElement.scrollWidth > innerWidth,
                cards: document.querySelectorAll('[data-menu-item]').length
            };
        })()`);
        if (state.images.some(image => !image.loaded || image.src.includes('menu-placeholder'))) throw new Error(`Missing image on ${page.path}`);
        if (state.horizontalOverflow || state.cards === 0) throw new Error(`Invalid layout or empty menu on ${page.path}`);
        const screenshot = await command('Page.captureScreenshot', { format: 'png', captureBeyondViewport: true });
        await writeFile(new URL(`${page.name}.png`, output), Buffer.from(screenshot.data, 'base64'));
        report.push({ path: page.path, file: `${page.name}.png`, viewport: `${page.width} × ${page.height}`, ...state });
        console.log(`${page.name}.png: ${state.cards} cards, ${state.images.length} images loaded`);
    }
    await writeFile(new URL('capture-report.json', output), `${JSON.stringify(report, null, 2)}\n`);
} finally {
    await command('Page.close').catch(() => {});
    socket.close();
}
