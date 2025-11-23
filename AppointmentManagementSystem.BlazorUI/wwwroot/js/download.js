window.downloadFileFromBase64 = (fileName, contentType, base64Data) => {
    try {
        const linkSource = `data:${contentType};base64,${base64Data}`;
        const downloadLink = document.createElement('a');
        downloadLink.href = linkSource;
        downloadLink.download = fileName || 'rapor.xlsx';
        downloadLink.click();
    } catch (error) {
        console.error('Dosya indirme sırasında hata oluştu', error);
    }
};
