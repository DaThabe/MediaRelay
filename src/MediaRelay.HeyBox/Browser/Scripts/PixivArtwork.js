async () => {

    // 等待图像加载
    const waitForImages = () => {
        return new Promise((resolve) => {
            const check = () => {
                const images = document.querySelectorAll('figure a');
                if (images.length > 0) {
                    resolve(true);
                } else {
                    setTimeout(check, 300);
                }
            };
            check();
        });
    };
    await waitForImages();


    // 展开图像
    const handleExpandButton = () => {
        return new Promise((resolve) => {
            const buttons = document.querySelectorAll('button');
            let targetBtn = null;

            for (const btn of buttons) {
                const text = btn.innerText || '';
                if (text.includes('查看全部') || text.includes('展开')) {
                    targetBtn = btn;
                    break;
                }
            }

            if (targetBtn) {
                // 模拟点击
                targetBtn.click();

                // 等待内容展开（简单延迟）
                setTimeout(resolve, 2000);
            } else {
                resolve();
            }
        });
    };
    await handleExpandButton();



    // 图像
    const resources = Array.from(document.querySelectorAll('figure a'))
        .map(img => img.getAttribute("href"))
        .filter(href => href && href.trim() !== "");

    // 正文
    const figcaption = document.querySelector("figcaption");
    // 标题
    const title = figcaption?.querySelector("h1")
        ?.innerText?.trim() || "";
    // 描述
    const describe = figcaption?.querySelector("p")
        ?.innerText?.trim() || "";
    // 上传时间
    const uploadAt = figcaption?.querySelector("time")
        ?.getAttribute("datetime") || "";
    // 标签
    const tags = Array.from(figcaption?.querySelectorAll("footer li a") || [])
        .map(a => a.innerText.trim())
        .filter(href => href && href.trim() !== "");

    // 作者
    const author = document?.querySelector("aside h2 div > div a");
    const authorName = author?.innerText.trim() || "";
    const authorPath = author?.getAttribute("href") || "";
    const authorUrl = authorPath ? `https://www.pixiv.net${authorPath}` : "";

    // 提取数据
    const result = {
        Resources: [... new Set(resources)],
        Title: title,
        Describe: describe,
        Tags: [... new Set(tags)],
        UploadAt: uploadAt,
        AuthorName: authorName,
        AuthorUrl: authorUrl
    };

    console.log(result);
    return JSON.stringify(result);
}