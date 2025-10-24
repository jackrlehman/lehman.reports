window.downloadFile = function(fileName, base64String) {
    const link = document.createElement('a');
    link.download = fileName;
    link.href = "data:application/pdf;base64," + base64String;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

window.downloadJson = function(fileName, jsonString) {
    const link = document.createElement('a');
    link.download = fileName;
    link.href = "data:application/json;charset=utf-8," + encodeURIComponent(jsonString);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

window.triggerFileUpload = function(elementId) {
    document.getElementById(elementId).click();
}
