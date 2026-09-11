async () => {

    // 等待图像加载
    const waitForImages = () => {
        return new Promise((resolve) => {
            const check = () => {
                const images = document.querySelectorAll('div.swiper-wrapper img');
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
            const button = document.querySelector('div.swiper-wrapper img');

            if (button) {
                // 模拟点击
                button.click();

                // 等待内容展开（简单延迟）
                setTimeout(resolve, 2000);
            } else {
                resolve();
            }
        });
    };
    await handleExpandButton();



    // 图像
    const resources = Array.from(document.querySelectorAll('div.panzoom img'))
        .map(img => img.getAttribute("src"))
        .filter(href => href && href.trim() !== "");

    // 正文
    const figcaption = document.querySelector("div.image-text__container");
    // 标题
    const title = figcaption?.querySelector("div.section-title__content")
        ?.innerText?.trim() || "";
    // 描述
    const describe = figcaption?.querySelector("div.image-text__content")
        ?.innerText?.trim() || "";
    // 上传时间
    const uploadAt = figcaption?.querySelector("div.link-data__time")
        ?.innerText?.trim() || "";
    // 标签
    const tags = Array.from(figcaption?.querySelectorAll("div.link-section-tags button") || [])
        .map(a => a.innerText.trim())
        .filter(href => href && href.trim() !== "");

    // 作者
    const author = document?.querySelector("div.link-section-user a");
    const authorName = author?.innerText.trim() || "";
    const authorPath = author?.getAttribute("href") || "";
    const authorUrl = authorPath ? `https://www.xiaoheihe.cn/${authorPath}` : "";

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