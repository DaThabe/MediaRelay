# MediaRelay (媒体转发)

![License](https://img.shields.io/github/license/DaThabe/MediaRelay)
![.NET](https://img.shields.io/badge/.NET-%3E%3D8.0-512BD4)
![Platform](https://img.shields.io/badge/platform-Windows-blue)

把多种来源的媒体转发至目标平台

## 媒体来源

### Pixiv Artworks (Pixiv-作品)

- 通过输入 `Url` 后使用 `Playwright Chrome` 自动抓取  
- 需要配置 `Pixiv:Http:Cookies` 下 `PHPSESSID` 的 Value

### Twitter Tweet (推特-推文)

- 通过输入 `Url` 后使用 `Playwright Chrome` 自动抓取  
- 支持图像和视频的下载
- 需要配置 `Twitter:Http:Cookies` 下 `auth_token` 的 Value

### HeyBox Bbs Link（小黑盒-社区帖子）

- 通过输入 `Url` 后使用 `Playwright Chrome` 自动抓取  
- 需要配置 `HeyBox:Http:Cookies` 下 `user_heybox_id`,`user_device_id`,`user_pkey`,`x_xhh_tokenid` 的 Value

## 转发目标

### Immich

- 通过 Immich Api 上传媒体
- 需要配置 `Immich:ApiKey`

## 输入方式

- **Console Url**：在控制台输入支持的网址后执行
- **Clipboard Url**：后台轮询剪贴板，检测到支持的网址后自动执行

## 工作原理

输入Url > 打开自动浏览器 > 根据脚本提取快照信息(资源链接,正文,标签,发布时间等等) > 转为标准转发内容 > 转发给目标平台

## 快速开始

1. 配置 `appsettings.json`
2. 运行 `MediaRelay.Launcher.exe`
3. 在控制台输入 URL，或复制 URL 到剪贴板

## 自己构建

### 前置要求

- .NET SDK >= 8.0
- Playwright Chromium 浏览器

### 准备浏览器

1. 下载 Playwright Chromium（或运行 `pwsh playwright.ps1 install chromium`）
2. 将浏览器放到任意目录
3. 在 `appsettings.json` 中指定 `chrome.exe` 的路径：`"Browser:Launch:ExecutablePath": "./Browser/chrome-win64/chrome.exe"`  

## 致谢

感谢以下开源项目：

- [Microsoft.Extensions.Hosting](https://github.com/dotnet/runtime) — 应用宿主与生命周期管理
- [Microsoft.Extensions.DependencyInjection](https://github.com/dotnet/runtime) — 依赖注入
- [Microsoft.Extensions.Configuration](https://github.com/dotnet/runtime) — 配置绑定
- [Microsoft.Extensions.Logging](https://github.com/dotnet/runtime) — 日志

- [Playwright.Clearcote](https://github.com/Clearcote/Playwright.Clearcote) — 支持 AOT 的 Playwright

- [Spectre.Console](https://github.com/spectreconsole/spectre.console) — 控制台增强
- [AsyncConsoleReader](https://github.com/... ) — 异步控制台输入
- [TextCopy](https://github.com/CopyText/TextCopy) — 剪贴板访问

- [Polly.Core](https://github.com/App-vNext/Polly) — 重试与容错
- [Apigen.Immich.Client](https://github.com/...) — Immich API 客户端

- [MSTest](https://github.com/microsoft/testfx) — 测试框架
- [Moq](https://github.com/devlooped/moq) — Mock 框架
- [Spectre.Console.Testing](https://github.com/spectreconsole/spectre.console) — 控制台测试
