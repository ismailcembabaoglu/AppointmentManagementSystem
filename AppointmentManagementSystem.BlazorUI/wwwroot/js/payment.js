window.paymentRedirect = {
    open: function (url) {
        if (!url) {
            return;
        }

        const dataPrefix = 'data:text/html;base64,';
        if (url.startsWith(dataPrefix)) {
            const base64Content = url.substring(dataPrefix.length);
            let html = '';
            try {
                html = atob(base64Content);
            } catch (e) {
                console.error('PayTR 3D HTML decode failed', e);
                window.location.href = url;
                return;
            }

            const popup = window.open('', '_blank');
            if (popup && !popup.closed) {
                popup.document.open();
                popup.document.write(html);
                popup.document.close();
            } else {
                const blob = new Blob([html], { type: 'text/html' });
                const blobUrl = URL.createObjectURL(blob);
                window.location.href = blobUrl;
            }
        } else {
            window.location.href = url;
        }
    }
};
