async () => {

    // 等待推文加载
    const waitForTweet = () => {
        return new Promise((resolve) => {
            const check = () => {
                const tweet = document.querySelector("article[data-testid='tweet'] div[data-testid='tweetPhoto']");
                if (tweet) {
                    resolve(true);
                } else {
                    setTimeout(check, 300);
                }
            };
            check();
        });
    };
    await waitForTweet();


    // 推文主体
    const tweet = document.querySelector("article[data-testid='tweet']");
    if (!tweet) return null;

    // 图片
    const resources = Array.from(tweet
        .querySelectorAll("div[data-testid='tweetPhoto'] img"))
        .map(img => img.src);

    // 正文
    const content = tweet.querySelector("[data-testid='tweetText']")
        ?.innerText?.trim() || "";

    // 标签（#标签）
    const tags = [content.matchAll(/#([^\s#]+)/g)]
        .map(m => m[1]);

    // 发布时间
    const uploadAt = tweet.querySelector("time")
        ?.getAttribute('datetime') || '';

    // 作者名称
    const author = tweet.querySelector("[data-testid='User-Name'] a");
    const authorPath = author?.getAttribute("href") || "";
    const authorUrl = authorPath ? `https://x.com${authorPath}` : "";
    const authorName = author.innerText?.trim() || "";

    const result =  {
        Resources: [...new Set(resources)],
        Content: content,
        UploadAt: uploadAt,
        AuthorName: authorName,
        AuthorUrl: authorUrl,
        Tags: [... new Set(tags)]
    };

    return JSON.stringify(result);
}