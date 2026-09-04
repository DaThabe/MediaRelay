async () => {
    // ============================================================
    // 1. 等待图片加载
    // ============================================================
    const waitForImages = () => {
        return new Promise((resolve) => {
            const checkImages = () => {
                const images = document.querySelectorAll('figure img');
                if (images.length > 0) {
                    // 检查图片是否加载完成
                    let loaded = true;
                    images.forEach(img => {
                        if (!img.complete) loaded = false;
                    });
                    if (loaded) {
                        resolve(true);
                        return;
                    }
                }
                setTimeout(checkImages, 500);
            };
            checkImages();
        });
    };

    // ============================================================
    // 2. 处理“查看全部/展开”按钮
    // ============================================================
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

    // ============================================================
    // 3. 滚动到底部（触发懒加载）
    // ============================================================
    const scrollToBottom = () => {
        return new Promise((resolve) => {
            window.scrollTo(0, document.body.scrollHeight);
            setTimeout(resolve, 1000);
        });
    };

    // ============================================================
    // 4. 提取数据
    // ============================================================
    const getVal = (sel) => {
        const el = document.querySelector(sel);
        return el ? el.innerText.trim() : "";
    };

    // ============================================================
    // 执行流程
    // ============================================================
    // 等待图片
    await waitForImages();

    // 点击展开
    await handleExpandButton();

    // 滚动到底部（触发懒加载）
    await scrollToBottom();

    // 再等一次图片加载
    await waitForImages();

    // 提取数据
    const result = {
        Resources: Array.from(document.querySelectorAll('figure img')).map(img => img.src),
        Title: getVal('figcaption h1'),
        Describe: getVal('figcaption p'),
        Tags: Array.from(document.querySelectorAll('figcaption footer li a')).map(a => a.innerText.trim()),
        UploadAt: document.querySelector('figcaption time')?.getAttribute('datetime') || '',
        AuthorName: getVal('aside h2 div > div a'),
        AuthorUrl: document.querySelector('aside h2 div > div a')?.getAttribute('href') || ''
    };

    return JSON.stringify(result);
}