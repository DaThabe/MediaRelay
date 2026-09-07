async (url) => {

    // 等待加载
    const waitForInput = () => {
        return new Promise((resolve) => {
            const check = () => {
                const tweet = document.querySelector("input[id='s_input']");
                if (tweet) {
                    resolve(true);
                } else {
                    setTimeout(check, 300);
                }
            };
            check();
        });
    };
    await waitForInput();


    // 输入框
    var inputBox = document.querySelector("input[id='s_input']");
    // 搜索按钮
    var searchButton = document.querySelector("button[class= 'btn-red']");

    if (!inputBox || !searchButton) return;

    // 输入后点击
    inputBox.value = url;
    searchButton.click();

    // 等待加载下载连接
    const waitForDownloadUrl = () => {
        return new Promise((resolve) => {
            const check = () => {
                const tweet = document.querySelector("div.download-section a");
                if (tweet) {
                    resolve(true);
                } else {
                    setTimeout(check, 300);
                }
            };
            check();
        });
    };
    await waitForDownloadUrl();

    // 返回
    return document.querySelector("div.download-section a")?.getAttribute("href") || "";
}