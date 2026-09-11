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

    // 时间解析
    function parseUploadAt(raw, now = new Date()) {
        if (!raw) return null;

        const text = raw.trim();

        // 尝试匹配：可选年份 + 月 + 日 + 可选时间
        // 2026-09-11 / 09-11 / 2026-09-11 12:30 / 09-11 12:30
        const match = text.match(
            /^(?:(\d{4})[-/])?(\d{1,2})[-/](\d{1,2})(?:\s+(\d{1,2}):(\d{1,2}))?$/
        );

        if (!match) return null;

        const year = match[1] ? parseInt(match[1], 10) : now.getFullYear();
        const month = parseInt(match[2], 10);
        const day = parseInt(match[3], 10);
        const hour = match[4] ? parseInt(match[4], 10) : 0;
        const minute = match[5] ? parseInt(match[5], 10) : 0;

        // 本地时间转 ISO
        const date = new Date(year, month - 1, day, hour, minute, 0);
        return date.toISOString();
    }



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
    const uploadAt = parseUploadAt(figcaption?.querySelector("div.link-data__time")
        ?.innerText?.trim() || "");
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