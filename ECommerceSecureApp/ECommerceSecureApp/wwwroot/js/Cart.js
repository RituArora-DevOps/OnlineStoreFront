(function () {
    function findToken(form) {
        // For ASP.NET Core antiforgery, hidden input name is "__RequestVerificationToken"
        var el = form.querySelector('input[name="__RequestVerificationToken"]');
        return el ? el.value : null;
    }
    async function postAddToCart(form) {
        const token = findToken(form);
        const fd = new FormData(form);
        const res = await fetch(form.action, {
            method: 'POST',
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                ...(token ? { 'RequestVerificationToken': token } : {})
            },
            body: fd,
            credentials: 'same-origin'
        });
        if (!res.ok) throw new Error('Add to cart failed');
        return res.text();
    }
    function showToast(message) {
        const body = document.getElementById('cartToastBody');
        if (body) body.textContent = message || 'Added to cart.';
        const toastEl = document.getElementById('cartToast');
        if (!toastEl) return;
        // Uses Bootstrap 5 Toast
        let toast = bootstrap.Toast.getOrCreateInstance(toastEl, { delay: 2000 });
        toast.show();
    }
    document.addEventListener('click', function (e) {
        const btn = e.target.closest('form.js-add-to-cart button[type="submit"]');
        if (!btn) return;
        const form = btn.closest('form.js-add-to-cart');
        if (!form) return;
        e.preventDefault();
        const name = form.getAttribute('data-product-name') || 'Item';
        postAddToCart(form)
            .then(() => showToast(`${name} added to cart`))
            .catch(() => showToast('Failed to add to cart'));
    });
})();
