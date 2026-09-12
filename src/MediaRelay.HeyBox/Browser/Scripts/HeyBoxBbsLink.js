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

        // 刚刚
        if (text === "刚刚") return now.toISOString();

        // n分钟前
        const minutesMatch = text.match(/^(\d+)\s*分钟前$/);
        if (minutesMatch) {
            const d = new Date(now.getTime() - parseInt(minutesMatch[1]) * 60 * 1000);
            return d.toISOString();
        }

        // n小时前
        const hoursMatch = text.match(/^(\d+)\s*小时前$/);
        if (hoursMatch) {
            const d = new Date(now.getTime() - parseInt(hoursMatch[1]) * 60 * 60 * 1000);
            return d.toISOString();
        }

        // n天前
        const daysMatch = text.match(/^(\d+)\s*天前$/);
        if (daysMatch) {
            const d = new Date(now.getTime() - parseInt(daysMatch[1]) * 24 * 60 * 60 * 1000);
            return d.toISOString();
        }

        // 日期格式：yyyy-MM-dd / MM-dd / yyyy-MM-dd HH:mm / MM-dd HH:mm
        const dateMatch = text.match(
            /^(?:(\d{4})[-/])?(\d{1,2})[-/](\d{1,2})(?:\s+(\d{1,2}):(\d{1,2}))?$/
        );
        if (dateMatch) {
            const year = dateMatch[1] ? parseInt(dateMatch[1], 10) : now.getFullYear();
            const month = parseInt(dateMatch[2], 10);
            const day = parseInt(dateMatch[3], 10);
            const hour = dateMatch[4] ? parseInt(dateMatch[4], 10) : 0;
            const minute = dateMatch[5] ? parseInt(dateMatch[5], 10) : 0;
            const d = new Date(year, month - 1, day, hour, minute, 0);
            return d.toISOString();
        }

        return null;
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