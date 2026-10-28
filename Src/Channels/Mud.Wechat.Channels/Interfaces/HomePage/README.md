# HomePage 域 · 主页管理

微信小店 / 视频号（channels 生态）「主页管理」域 SDK（14 端点：主页商品排序 / 主页背景图 / 精选展示位 / 店铺主页商品分类）。 接口：[IChannelsHomePageService](./IChannelsHomePageService.cs)；注册组 `HomePage`；模块枚举 `ChannelsModule.HomePage`（装配入口见 [ChannelsServiceBuilder.cs](../../Extensions/ChannelsServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[ChannelsHomePageContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsHomePageContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Channels/HomePage`](../../../Mud.Wechat.Channels/HomePage/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IChannelsHomePageService = 14；按 HTTP 特性实计 **14** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IChannelsHomePageService` | `GetStoreWindowProductListAsync` | POST | `/channels/ec/store/window/product/list/get` | `AccessToken`（Query），Query `access_token` | 获取主页展示商品列表（`getstorewindowproductlist`，仅返回上架中的商品）。 |
| 2 | `IChannelsHomePageService` | `ReorderStoreWindowProductAsync` | POST | `/channels/ec/store/window/product/reorder` | `AccessToken`（Query），Query `access_token` | 重新排序主页展示商品（`reorderstorewindowproduct`）。 |
| 3 | `IChannelsHomePageService` | `HideStoreWindowProductAsync` | POST | `/channels/ec/store/window/product/hide` | `AccessToken`（Query），Query `access_token` | 隐藏小店主页商品（`hidestorewindowproduct`，设置隐藏或取消隐藏）。 |
| 4 | `IChannelsHomePageService` | `SetTopStoreWindowProductAsync` | POST | `/channels/ec/store/window/product/settop` | `AccessToken`（Query），Query `access_token` | 置顶小店主页商品（`settopstorewindowproduct`，设置置顶或取消置顶）。 |
| 5 | `IChannelsHomePageService` | `SubmitBackgroundApplyAsync` | POST | `/channels/ec/basics/homepage/background/apply/submit` | `AccessToken`（Query），Query `access_token` | 提交主页背景图申请（`submitbackgroundapply`，审核通过后背景图展示在店铺主页顶部，按 16:9 比例）。 |
| 6 | `IChannelsHomePageService` | `GetBackgroundAsync` | POST | `/channels/ec/basics/homepage/background/get` | `AccessToken`（Query），Query `access_token` | 查询背景图（`getbackground`，返回当前生效背景图与流程中的申请）。 |
| 7 | `IChannelsHomePageService` | `RemoveBackgroundAsync` | POST | `/channels/ec/basics/homepage/background/remove` | `AccessToken`（Query），Query `access_token` | 清空主页背景图并撤销流程中的申请（`removebackground`）。 |
| 8 | `IChannelsHomePageService` | `CancelBackgroundApplyAsync` | POST | `/channels/ec/basics/homepage/background/apply/cancel` | `AccessToken`（Query），Query `access_token` | 撤销主页背景图申请（`cancelbackgroundapply`，仅撤销审核中的申请，不影响已生效背景图）。 |
| 9 | `IChannelsHomePageService` | `SubmitBannerApplyAsync` | POST | `/channels/ec/basics/homepage/banner/apply/submit` | `AccessToken`（Query），Query `access_token` | 提交精选展示位申请（`submitbannerapply`，审核通过后展示在店铺主页）。 |
| 10 | `IChannelsHomePageService` | `GetBannerAsync` | POST | `/channels/ec/basics/homepage/banner/get` | `AccessToken`（Query），Query `access_token` | 查询精选展示位（`getbanner`，返回当前生效展示位与流程中的申请）。 |
| 11 | `IChannelsHomePageService` | `RemoveBannerAsync` | POST | `/channels/ec/basics/homepage/banner/remove` | `AccessToken`（Query），Query `access_token` | 清空精选展示位并撤销流程中的申请（`removebanner`）。 |
| 12 | `IChannelsHomePageService` | `CancelBannerApplyAsync` | POST | `/channels/ec/basics/homepage/banner/apply/cancel` | `AccessToken`（Query），Query `access_token` | 撤销精选展示位申请（`cancelbannerapply`，仅撤销审核中的申请，不影响已生效展示位）。 |
| 13 | `IChannelsHomePageService` | `GetClassificationTreeAsync` | POST | `/channels/ec/store/classification/tree/get` | `AccessToken`（Query），Query `access_token` | 获取在店铺主页展示的商品分类（`getclassificationtree`，仅分类树，不含分类与商品的关联数据，支持 2 级树结构）。 |
| 14 | `IChannelsHomePageService` | `GetClassificationProductListAsync` | POST | `/channels/ec/store/classification/tree/product/get` | `AccessToken`（Query），Query `access_token` | 获取分类关联的商品ID列表（`getclassificationproductlist`）。 |

## 令牌与认证

本域全部端点消费 `Wechat.Channels.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**域范围（四组）**：① 主页商品排序（`/channels/ec/store/window/product/*`，4 端点）；
② 主页背景图（`/channels/ec/basics/homepage/background/*`，4 端点）；③ 精选展示位
（`/channels/ec/basics/homepage/banner/*`，4 端点）；④ 店铺主页商品分类
（`/channels/ec/store/classification/*`，2 端点）。

**与「视频号橱窗」严格区分（同族不同前缀，设计方案 v1 §4.3）**：本域 `/channels/ec/store/window/*`
是**微信小店店铺主页**对用户展示的商品；视频号个人橱窗走 `/channels/ec/window/product/*`
（见 `IChannelsWindowService`），官方文档亦分属两套 API 目录——两组端点各自照官方原文落位，
不得合并、不得互相替代。

**审核流语义**：背景图与精选展示位为「提交申请 → 审核 → 生效」模型，查询接口同时返回生效内容与
流程中的申请；「撤销申请」不影响已生效内容，「清空」接口才同时移除生效内容。

MUD005 已知接受风险：微信小店官方契约强制令牌走 Query 参数（`access_token`），
无法改用 Header；库内遥测与异常消息的 URL 已由组件 `SensitiveUrlRedactor` 与
`WechatChannelsException` 构造期脱敏处置。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 5 次）第三方平台代调用所属权限集 id 为 `129`；不支持云调用；仅「微信小店」账号类型可调用。
- （出现 14 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。
- （出现 8 次）第三方平台代调用所属权限集 id 为 `129`、`131`、`192`；不支持云调用。

### 1. `GetStoreWindowProductListAsync` — 获取主页展示商品列表（`getstorewindowproductlist`，仅返回上架中的商品）。

- **声明接口**：[IChannelsHomePageService](./IChannelsHomePageService.cs)
- **官方契约**：`POST /channels/ec/store/window/product/list/get`

- **请求体**：`ChannelsGetStoreWindowProductListRequest` — 查询请求（`page_size` 必填，默认 10 且不超过 30；`next_key` 翻页上下文）。
- **返回**：`ChannelsGetStoreWindowProductListResponse` — 主页展示商品列表（`products` 内含 `product_id`/`is_set_hide`/`is_set_top`）与翻页信息（`next_key`/`total_num`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/homepage/storewindow/api_getstorewindowproductlist
- 官方业务限制：①仅返回**上架中**的商品；②隐藏商品只在主页不可见，并不代表下架； / ③专属错误码 `10020050`（无权限调用）/ `10020051`（参数有误）。

### 2. `ReorderStoreWindowProductAsync` — 重新排序主页展示商品（`reorderstorewindowproduct`）。

- **声明接口**：[IChannelsHomePageService](./IChannelsHomePageService.cs)
- **官方契约**：`POST /channels/ec/store/window/product/reorder`

- **请求体**：`ChannelsReorderStoreWindowProductRequest` — 排序请求（`product_id`/`index_num` 必填）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/homepage/storewindow/api_reorderstorewindowproduct
- 官方业务限制：`index_num` 为商品重新排序后的新序号，**最大移动步长为 500**；本接口无特殊注意事项。

### 3. `HideStoreWindowProductAsync` — 隐藏小店主页商品（`hidestorewindowproduct`，设置隐藏或取消隐藏）。

- **声明接口**：[IChannelsHomePageService](./IChannelsHomePageService.cs)
- **官方契约**：`POST /channels/ec/store/window/product/hide`

- **请求体**：`ChannelsHideStoreWindowProductRequest` — 隐藏请求（`product_id`/`is_set_hide` 必填，1=隐藏 0=取消隐藏）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/homepage/storewindow/api_hidestorewindowproduct
- 官方业务限制：①隐藏后的商品**不会下架**，仍可通过其他渠道购买；②专属错误码 `10020052`（商品不存在）、 / `10020298`（相似商品经营受限，不允许取消隐藏）、`10020341`（商品正在参与平台活动，编辑前需退出活动）。

### 4. `SetTopStoreWindowProductAsync` — 置顶小店主页商品（`settopstorewindowproduct`，设置置顶或取消置顶）。

- **声明接口**：[IChannelsHomePageService](./IChannelsHomePageService.cs)
- **官方契约**：`POST /channels/ec/store/window/product/settop`

- **请求体**：`ChannelsSetTopStoreWindowProductRequest` — 置顶请求（`product_id`/`is_set_top` 必填，1=置顶 0=取消置顶）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/homepage/storewindow/api_settopstorewindowproduct
- 官方业务限制：本接口无特殊注意事项；专属错误码 `10020050`（无权限）/ `10020051`（参数有误）。

### 5. `SubmitBackgroundApplyAsync` — 提交主页背景图申请（`submitbackgroundapply`，审核通过后背景图展示在店铺主页顶部，按 16:9 比例）。

- **声明接口**：[IChannelsHomePageService](./IChannelsHomePageService.cs)
- **官方契约**：`POST /channels/ec/basics/homepage/background/apply/submit`

- **请求体**：`ChannelsSubmitHomepageBackgroundApplyRequest` — 申请请求（`img_url` 必填，须用「上传图片」接口 resp_type=1 返回的 img_url）。
- **返回**：`ChannelsHomepageApplyResponse` — 申请编号（`apply_id`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/homepage/background/api_submitbackgroundapply
- 官方业务限制：①仅支持上传**一张**背景图，按 16:9 比例展示在主页顶部；②图片支持 bmp、jpg/jpeg、png、svg、webp / 格式，上传大小上限 10MB；③图片须用「上传图片」接口（resp_type=1）获取，不接受其他格式的图片 url / （前缀 `mmecimage.cn/p/` 的已转换 url 可直接提交）；④已有流程中的申请时不可重复提交 / （`268549841`）。

### 6. `GetBackgroundAsync` — 查询背景图（`getbackground`，返回当前生效背景图与流程中的申请）。

- **声明接口**：[IChannelsHomePageService](./IChannelsHomePageService.cs)
- **官方契约**：`POST /channels/ec/basics/homepage/background/get`
- **返回**：`ChannelsGetHomepageBackgroundResponse` — 当前生效背景图（`img_url`）与最近一次流程中的申请（`apply`，已生效/已撤销的申请不返回）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/homepage/background/api_getbackground
- 官方契约：**POST** + 空请求体（官方参数表无 Body 参数）。 / 官方业务限制：专属错误码 `268549676`（既无生效中的主页背景图，也无流程中的申请）、 / `268555516`（无权限，仅供微信小店使用）。

### 7. `RemoveBackgroundAsync` — 清空主页背景图并撤销流程中的申请（`removebackground`）。

- **声明接口**：[IChannelsHomePageService](./IChannelsHomePageService.cs)
- **官方契约**：`POST /channels/ec/basics/homepage/background/remove`
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/homepage/background/api_removebackground
- 官方契约：**POST** + 空请求体（官方参数表无 Body 参数）。 / 官方业务限制：本接口**同时移除已生效内容并撤销审核中的申请**；若仅撤销申请而不影响已生效背景图， / 应改用 `CancelBackgroundApplyAsync`。

### 8. `CancelBackgroundApplyAsync` — 撤销主页背景图申请（`cancelbackgroundapply`，仅撤销审核中的申请，不影响已生效背景图）。

- **声明接口**：[IChannelsHomePageService](./IChannelsHomePageService.cs)
- **官方契约**：`POST /channels/ec/basics/homepage/background/apply/cancel`

- **请求体**：`ChannelsHomepageApplyIdRequest` — 撤销请求（`apply_id` 必填，申请编号）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/homepage/background/api_cancelbackgroundapply
- 官方业务限制：①仅能撤销**流程中（审核中）**的申请（`268549848`），不影响当前已生效的背景图； / ②专属错误码 `268549835`（未能找到申请）/ `268549854`（申请编号为空）。

### 9. `SubmitBannerApplyAsync` — 提交精选展示位申请（`submitbannerapply`，审核通过后展示在店铺主页）。

- **声明接口**：[IChannelsHomePageService](./IChannelsHomePageService.cs)
- **官方契约**：`POST /channels/ec/basics/homepage/banner/apply/submit`

- **请求体**：`ChannelsSubmitBannerApplyRequest` — 申请请求（`banner` 模块必填：展示位列表最多 5 个（小图模式最少 3 个）、`scale` 展示样式必填、模块标题选填）。
- **返回**：`ChannelsHomepageApplyResponse` — 申请编号（`apply_id`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/homepage/banner/api_submitbannerapply
- 官方业务限制：①展示位图片同样须用「上传图片」接口（resp_type=1）获取，不接受其他格式 url； / ②**type=商品/视频号/公众号三选一，只能设置对应类型的参数**（错配报参数错误）；③展示位标题长度受 / `scale` 约束（小图模式 ≤12 字、大图模式 ≤40 字，大图模式可设 ≤100 字描述，小图模式描述必须为空）； / ④已有流程中的申请时不可重复提交（`268549841`）；⑤视频号 ID / 视频 ID 可在视频号助手首页与 / 「动态管理」模块获取，且二者必须匹配（`268555694`）。

### 10. `GetBannerAsync` — 查询精选展示位（`getbanner`，返回当前生效展示位与流程中的申请）。

- **声明接口**：[IChannelsHomePageService](./IChannelsHomePageService.cs)
- **官方契约**：`POST /channels/ec/basics/homepage/banner/get`
- **返回**：`ChannelsGetBannerResponse` — 当前生效展示位（`banner`）与最近一次流程中的申请（`apply`，不返回已生效或已撤销的申请）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/homepage/banner/api_getbanner
- 官方契约：**POST** + 空请求体（官方参数表无 Body 参数）。 / 官方业务限制：专属错误码 `268549675`（既无生效中的精选展示位，也无流程中的申请）、 / `268555516`（无权限，仅供微信小店使用）。

### 11. `RemoveBannerAsync` — 清空精选展示位并撤销流程中的申请（`removebanner`）。

- **声明接口**：[IChannelsHomePageService](./IChannelsHomePageService.cs)
- **官方契约**：`POST /channels/ec/basics/homepage/banner/remove`
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/homepage/banner/api_removebanner
- 官方契约：**POST** + 空请求体（官方参数表无 Body 参数）。 / 官方业务限制：本接口**同时移除已生效展示位内容并撤销审核中的申请**；若仅撤销申请而不影响已生效展示位， / 应改用 `CancelBannerApplyAsync`。

### 12. `CancelBannerApplyAsync` — 撤销精选展示位申请（`cancelbannerapply`，仅撤销审核中的申请，不影响已生效展示位）。

- **声明接口**：[IChannelsHomePageService](./IChannelsHomePageService.cs)
- **官方契约**：`POST /channels/ec/basics/homepage/banner/apply/cancel`

- **请求体**：`ChannelsHomepageApplyIdRequest` — 撤销请求（`apply_id` 必填，申请编号）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/homepage/banner/api_cancelbannerapply
- 官方业务限制：①仅能撤销**流程中（审核中）**的申请（`268549848`），不影响当前已生效的展示位； / ②专属错误码 `268549835`（未能找到申请）/ `268549854`（申请编号为空）。

### 13. `GetClassificationTreeAsync` — 获取在店铺主页展示的商品分类（`getclassificationtree`，仅分类树，不含分类与商品的关联数据，支持 2 级树结构）。

- **声明接口**：[IChannelsHomePageService](./IChannelsHomePageService.cs)
- **官方契约**：`POST /channels/ec/store/classification/tree/get`
- **返回**：`ChannelsGetClassificationTreeResponse` — 分类树（`tree`：一级/二级分类列表、树 ID、创建/更新时间）与分类树版本号（`version`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/homepage/shoptype/api_getclassificationtree
- 官方契约：**POST** + 空请求体（官方参数表无 Body 参数）。 / 官方业务限制：①仅支持 2 级树结构，且**不含分类与商品的关联数据**（关联商品用 / `GetClassificationProductListAsync`）；②专属错误码 `10020241`（该接口仅支持微信小店调用，请升级后使用）。

### 14. `GetClassificationProductListAsync` — 获取分类关联的商品ID列表（`getclassificationproductlist`）。

- **声明接口**：[IChannelsHomePageService](./IChannelsHomePageService.cs)
- **官方契约**：`POST /channels/ec/store/classification/tree/product/get`

- **请求体**：`ChannelsGetClassificationProductListRequest` — 查询请求（`req` 对象必填：`level_1_id`/`level_2_id`/`page_context`/`page_size` 均必填，分类 ID 经分类树接口获取）。
- **返回**：`ChannelsGetClassificationProductListResponse` — 分类关联的商品 ID 列表（`product_ids`）与翻页上下文（`page_context`，为空表示已到最后一页）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/homepage/shoptype/api_getclassificationproductlist
- 官方业务限制：①**一个分类最多关联 500 个商品**；②本接口**不支持第三方平台代调用**（官方适用范围表未放行）； / ③请求体为嵌套形态 `{"req":{…}}`（官方参数表原文，照抄建模）。
- 不支持云调用。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Channels/HomePage`](../../../Mud.Wechat.Channels/HomePage/) 内共 **27** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **27** 个，`SerializerClassName` = `HomePage`。

AOT 源生成上下文：[`HomePageJsonContext.g.cs`](./../../../Mud.Wechat.Channels/Generated/HomePageJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `ChannelsGetStoreWindowProductListResponse` | 应答 | [HomePage/WindowProduct/ChannelsGetStoreWindowProductListResponse.cs](../../../Mud.Wechat.Channels/HomePage/WindowProduct/ChannelsGetStoreWindowProductListResponse.cs) | `products`, `next_key`, `total_num` |
| `ChannelsGetStoreWindowProductListRequest` | 请求体 | [HomePage/WindowProduct/ChannelsGetStoreWindowProductListRequest.cs](../../../Mud.Wechat.Channels/HomePage/WindowProduct/ChannelsGetStoreWindowProductListRequest.cs) | `page_size`, `next_key` |
| `ChannelsResponse` | 应答 | [ChannelsResponse.cs](../../../Mud.Wechat.Channels/ChannelsResponse.cs) | `errcode`, `errmsg` |
| `ChannelsReorderStoreWindowProductRequest` | 请求体 | [HomePage/WindowProduct/ChannelsReorderStoreWindowProductRequest.cs](../../../Mud.Wechat.Channels/HomePage/WindowProduct/ChannelsReorderStoreWindowProductRequest.cs) | `product_id`, `index_num` |
| `ChannelsHideStoreWindowProductRequest` | 请求体 | [HomePage/WindowProduct/ChannelsHideStoreWindowProductRequest.cs](../../../Mud.Wechat.Channels/HomePage/WindowProduct/ChannelsHideStoreWindowProductRequest.cs) | `product_id`, `is_set_hide` |
| `ChannelsSetTopStoreWindowProductRequest` | 请求体 | [HomePage/WindowProduct/ChannelsSetTopStoreWindowProductRequest.cs](../../../Mud.Wechat.Channels/HomePage/WindowProduct/ChannelsSetTopStoreWindowProductRequest.cs) | `product_id`, `is_set_top` |
| `ChannelsHomepageApplyResponse` | 应答 | [HomePage/Apply/ChannelsHomepageApplyResponse.cs](../../../Mud.Wechat.Channels/HomePage/Apply/ChannelsHomepageApplyResponse.cs) | `apply_id` |
| `ChannelsSubmitHomepageBackgroundApplyRequest` | 请求体 | [HomePage/Background/ChannelsSubmitHomepageBackgroundApplyRequest.cs](../../../Mud.Wechat.Channels/HomePage/Background/ChannelsSubmitHomepageBackgroundApplyRequest.cs) | `img_url` |
| `ChannelsGetHomepageBackgroundResponse` | 应答 | [HomePage/Background/ChannelsGetHomepageBackgroundResponse.cs](../../../Mud.Wechat.Channels/HomePage/Background/ChannelsGetHomepageBackgroundResponse.cs) | `img_url`, `apply` |
| `ChannelsHomepageApplyIdRequest` | 请求体 | [HomePage/Apply/ChannelsHomepageApplyIdRequest.cs](../../../Mud.Wechat.Channels/HomePage/Apply/ChannelsHomepageApplyIdRequest.cs) | `apply_id` |
| `ChannelsSubmitBannerApplyRequest` | 请求体 | [HomePage/Banner/ChannelsSubmitBannerApplyRequest.cs](../../../Mud.Wechat.Channels/HomePage/Banner/ChannelsSubmitBannerApplyRequest.cs) | `banner` |
| `ChannelsGetBannerResponse` | 应答 | [HomePage/Banner/ChannelsGetBannerResponse.cs](../../../Mud.Wechat.Channels/HomePage/Banner/ChannelsGetBannerResponse.cs) | `banner`, `apply` |
| `ChannelsGetClassificationTreeResponse` | 应答 | [HomePage/Classification/ChannelsGetClassificationTreeResponse.cs](../../../Mud.Wechat.Channels/HomePage/Classification/ChannelsGetClassificationTreeResponse.cs) | `tree`, `version` |
| `ChannelsGetClassificationProductListResponse` | 应答 | [HomePage/Classification/ChannelsGetClassificationProductListResponse.cs](../../../Mud.Wechat.Channels/HomePage/Classification/ChannelsGetClassificationProductListResponse.cs) | `product_ids`, `page_context` |
| `ChannelsGetClassificationProductListRequest` | 请求体 | [HomePage/Classification/ChannelsGetClassificationProductListRequest.cs](../../../Mud.Wechat.Channels/HomePage/Classification/ChannelsGetClassificationProductListRequest.cs) | `req` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Channels;
using Mud.Wechat.Channels.DataModels.HomePage;

builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddHomePageApi());

var service = app.Services.GetRequiredService<IChannelsHomePageService>();
var ct = CancellationToken.None;
var response = await service.GetBackgroundAsync(ct);
// 业务失败判定：WechatChannelsException.ThrowIfFailed(response, "/channels/ec/basics/homepage/background/get");
// 占位值仅用于编译示意；业务值须按 IChannelsHomePageService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`GetBackgroundAsync`）；Path / Query 位置参数与官方必填口径见「端点契约明细」。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IChannelsHomePageService](./IChannelsHomePageService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`ChannelsHomePageContractGuards.cs`](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsHomePageContractGuards.cs) |
| 守卫中的代表性测试方法 | `HomePageEndpoints_ShouldMatchOfficialRoutes`、`HomePageEndpoints_ShouldAllBePost_WithEmptyBodyRules`、`HomePageInterface_ShouldBindChannelsToken_AndRegisterUnderHomePageGroup`、`HomePageDataModels_ShouldBeRegisteredInJsonContext`、`HomePageDataModels_ShouldLockOfficialFieldsAndSharedDtos`、`HomePageEnumConstants_ShouldLockOfficialLiteralValues`、`RemovedClassificationWriteRoutes_ShouldHaveZeroDeclaration` |
| 模块枚举 / 装配入口 | [`ChannelsModule.cs`](../../Extensions/ChannelsModule.cs)、[`ChannelsServiceBuilder.cs`](../../Extensions/ChannelsServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Channels/HomePage`](../../../Mud.Wechat.Channels/HomePage/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Channels/Mud.Wechat.Channels/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`ChannelsHomePageContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `Wechat.Channels.AccessToken`；接口平铺在 `Mud.Wechat.Channels`，不得声明 `IsAbstract` 父接口（CH-X2）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
