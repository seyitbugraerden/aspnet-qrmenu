document.querySelectorAll('[data-fallback]').forEach(image => {
    const useFallback = () => {
        image.removeEventListener('error', useFallback);
        image.src = '/images/menu-placeholder.svg';
    };
    image.addEventListener('error', useFallback);
    if (image.complete && image.naturalWidth === 0) useFallback();
});

const search = document.querySelector('[data-menu-search]');
if (search) {
    const items = [...document.querySelectorAll('[data-menu-item]')];
    const normalize = value => value.toLocaleLowerCase('tr-TR').normalize('NFD').replace(/[\u0300-\u036f]/g, '').replace(/ı/g, 'i');
    search.addEventListener('input', () => {
        const query = normalize(search.value.trim());
        let visible = 0;
        items.forEach(item => {
            item.hidden = !normalize(item.dataset.name).includes(query);
            if (!item.hidden) visible++;
        });
        document.querySelector('[data-search-empty]').hidden = visible > 0 || !query;
        document.querySelector('[data-search-status]').textContent = `${visible} sonuç bulundu.`;
    });
}
