# Window 域 · 橱窗与本地生活商品

微信小店 / 视频号（channels 生态）「橱窗与本地生活商品」域 SDK（9 端点：视频号橱窗商品 4 / 本地生活团购优惠 5）。 接口：[IChannelsWindowService](./IChannelsWindowService.cs)；注册组 `Window`；模块枚举 `ChannelsModule.Window`（装配入口见 [ChannelsServiceBuilder.cs](../../Extensions/ChannelsServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[ChannelsWindowContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsWindowContractGuards.cs)、[ChannelsTalentContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsTalentContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Channels.DataModels/Window`](../../../Mud.Wechat.Channels.DataModels/Window/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IChannelsWindowService = 9；按 HTTP 特性实计 **9** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IChannelsWindowService` | `AddWindowProductAsync` | POST | `/channels/ec/window/product/add` | `AccessToken`（Query），Query `access_token` | 上架商品到橱窗（`addwindowproduct`；官方注明该 API 已无实际作用——自营商品上下架状态自动同步）。 |
| 2 | `IChannelsWindowService` | `GetWindowProductAsync` | POST | `/channels/ec/window/product/get` | `AccessToken`（Query），Query `access_token` | 获取橱窗商品详情（`getwindowproduct`，可查自营与带货商品）。 |
| 3 | `IChannelsWindowService` | `GetWindowProductListAsync` | POST | `/channels/ec/window/product/list/get` | `AccessToken`（Query），Query `access_token` | 获取已添加到橱窗的商品列表（`getwindowproductlist`）。 |
| 4 | `IChannelsWindowService` | `OffWindowProductAsync` | POST | `/channels/ec/window/product/off` | `AccessToken`（Query），Query `access_token` | 下架橱窗商品（`offwindowproduct`；官方注明该 API 已无实际作用——自营商品上下架状态自动同步）。 |
| 5 | `IChannelsWindowService` | `AddLocallifeProductAsync` | POST | `/channels/ec/product/locallife/add` | `AccessToken`（Query），Query `access_token` | 添加团购优惠（`addlocalproduct`，创建本地团购优惠；`product_type`/`code_source_type` 创建后不可改）。 |
| 6 | `IChannelsWindowService` | `GetLocallifeProductListAsync` | POST | `/channels/ec/product/locallife/list/get` | `AccessToken`（Query），Query `access_token` | 查询团购优惠列表（`getlocalproductlist`，不填 status 默认拉全部商品，不包含回收站）。 |
| 7 | `IChannelsWindowService` | `GetLocallifeProductAsync` | POST | `/channels/ec/product/locallife/get` | `AccessToken`（Query），Query `access_token` | 查询团购优惠详情（`getlocalproduct`，可分别获取线上数据与草稿数据）。 |
| 8 | `IChannelsWindowService` | `AuditFreeUpdateLocallifeProductAsync` | POST | `/channels/ec/product/locallife/auditfree` | `AccessToken`（Query），Query `access_token` | 免审更新团购优惠（`updatelocalauditfree`，针对已上线的团购优惠免审核更新售卖价格）。 |
| 9 | `IChannelsWindowService` | `UpdateLocallifeProductAsync` | POST | `/channels/ec/product/locallife/update` | `AccessToken`（Query），Query `access_token` | 更新团购优惠（`updatelocalproduct`，全量更新；更新成功后通过「团购优惠更新回调」通知商家）。 |

## 令牌与认证

本域全部端点消费 `Wechat.Channels.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**域范围（两组，同域不同前缀）**：① 视频号橱窗商品（`/channels/ec/window/product/*`，4 端点，
视频号助手 API，凭据为视频号助手 access_token）；② 本地生活团购优惠（`/channels/ec/product/locallife/*`，
5 端点，微信小店商家管理商品面）。两组端点照官方原文各自落位，不合并不重复。

**与小店「主页商品排序」严格区分（同族不同前缀）**：本域的 `/channels/ec/window/*` 是
**视频号个人橱窗**；微信小店店铺主页展示商品走 `/channels/ec/store/window/*`
（见 `IChannelsHomePageService`），不得合并或混用。

**官方契约警示（照录）**：橱窗上下架接口（add/off）官方注明「由于目前橱窗中自营商品的上下架状态会自动同步为
商品在店铺中的状态，该 api 已无实际作用」——保留端点以对齐官方开放面，调用方请优先走小店商品上下架。

MUD005 已知接受风险：微信小店官方契约强制令牌走 Query 参数（`access_token`），
无法改用 Header；库内遥测与异常消息的 URL 已由组件 `SensitiveUrlRedactor` 与
`WechatChannelsException` 构造期脱敏处置。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 9 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。
- （出现 5 次）第三方平台代调用所属权限集 id 为 `129`；不支持云调用；仅「微信小店」账号类型可调用。

### 1. `AddWindowProductAsync` — 上架商品到橱窗（`addwindowproduct`；官方注明该 API 已无实际作用——自营商品上下架状态自动同步）。

- **声明接口**：[IChannelsWindowService](./IChannelsWindowService.cs)
- **官方契约**：`POST /channels/ec/window/product/add`

- **请求体**：`ChannelsAddWindowProductRequest` — 上架请求（`product_id`/`appid` 必填；`is_hide_for_window` 选填，默认 false）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/channels/api/channels/windowproduct/api_addwindowproduct
- 官方业务限制：①**不支持带货中心来源的商品**，其余商品的橱窗商品 ID 与商品来源处的平台内部商品 ID 相同； / ②官方错误码 `10022001`（验证视频号身份失败）/ `10022002`（橱窗被禁止使用）/ / `10022004`（商品异常禁止添加）/ `10022005`（橱窗商品数超过 1 万）/ `10022007`（不支持操作带货中心来源商品）/ / `10022008`（请求的 appid 不属于该视频号的绑定店铺）；③仅「视频号助手」账号类型可调用。
- **官方文档矛盾照录**：参数表 `product_id` 标注 number、请求示例为带引号字符串——SDK 按参数表建模； / 而同域 get/off/list 三端点的参数表均为 string(uint64)。
- 第三方平台代调用所属权限集 id 为 `133`、`177`；支持云调用。

### 2. `GetWindowProductAsync` — 获取橱窗商品详情（`getwindowproduct`，可查自营与带货商品）。

- **声明接口**：[IChannelsWindowService](./IChannelsWindowService.cs)
- **官方契约**：`POST /channels/ec/window/product/get`

- **请求体**：`ChannelsGetWindowProductRequest` — 查询请求（`product_id`/`appid` 必填——`need_cps_product`=true 时 appid 可留空；`need_cps_product` 选填）。
- **返回**：`ChannelsGetWindowProductResponse` — 自营商品详情（`product`）与带货商品列表（`cps_products`，带货计划类型 `cps_item_type`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/channels/api/channels/windowproduct/api_getwindowproduct
- 官方业务限制：①自营商品橱窗 ID 即小店商品 ID，带货商品橱窗 ID 为货源商品在小店的商品 ID / （同一货源商品可能对应多个带货商品，每个带货商品 ID 唯一）；②填 `need_cps_product`=true 后 / `product_id` 支持两种 ID 查询——输入小店商品 ID 返回其全部带货计划，输入带货商品 ID 只返回该带货商品的计划； / ③官方注明 `leaf_category_id`（叶子类目 ID）将替代 `third_category_id`，后者最终会下掉，需尽快迁移。
- 第三方平台代调用所属权限集 id 为 `133`、`177`；支持云调用；仅「视频号助手」账号类型可调用。

### 3. `GetWindowProductListAsync` — 获取已添加到橱窗的商品列表（`getwindowproductlist`）。

- **声明接口**：[IChannelsWindowService](./IChannelsWindowService.cs)
- **官方契约**：`POST /channels/ec/window/product/list/get`

- **请求体**：`ChannelsGetWindowProductListRequest` — 查询请求（`page_size` 必填且不超过 200；其余选填，`appid` 与 `query_cps_product` 互斥——都传时以 appid 优先，仅返回来源指定店铺的自营商品）。
- **返回**：`ChannelsGetWindowProductListResponse` — 橱窗商品列表（`products`）与翻页信息（`last_buffer` 顺序翻页上下文 / `total_num` 商品总数）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/channels/api/channels/windowproduct/api_getwindowproductlist
- 官方业务限制：①**page_size × page_index ≤ 10000**，命中限制时改用 `last_buffer` 顺序翻页 / （传入 last_buffer 后 page_index 不生效）；②专属错误码 `10022006`（请求参数错误，请检查是否符合限制）、 / `48001`（接口权限未开启）。
- 第三方平台代调用所属权限集 id 为 `133`、`177`；支持云调用；仅「视频号助手」账号类型可调用。

### 4. `OffWindowProductAsync` — 下架橱窗商品（`offwindowproduct`；官方注明该 API 已无实际作用——自营商品上下架状态自动同步）。

- **声明接口**：[IChannelsWindowService](./IChannelsWindowService.cs)
- **官方契约**：`POST /channels/ec/window/product/off`

- **请求体**：`ChannelsOffWindowProductRequest` — 下架请求（`product_id`/`appid` 必填）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/channels/api/channels/windowproduct/api_offwindowproduct
- 官方业务限制：①**不支持带货中心来源的商品**；②专属错误码 `10022003`（橱窗商品未找到）/ / `10022007`（不支持操作带货中心来源商品）/ `10022008`（appid 不属于绑定店铺）；③仅「视频号助手」账号类型可调用。
- 第三方平台代调用所属权限集 id 为 `133`、`177`；支持云调用。

### 5. `AddLocallifeProductAsync` — 添加团购优惠（`addlocalproduct`，创建本地团购优惠；`product_type`/`code_source_type` 创建后不可改）。

- **声明接口**：[IChannelsWindowService](./IChannelsWindowService.cs)
- **官方契约**：`POST /channels/ec/product/locallife/add`

- **请求体**：`ChannelsAddLocallifeProductRequest` — 创建请求（`product_type`/`product_name`/`category_id`/`brand_id`/`head_imgs`/`code_source_type`/`attr_kv_map`/`skus` 必填）。
- **返回**：`ChannelsAddLocallifeProductResponse` — 平台生成的商品 ID（`data.product_id`）与 SKU ID 列表（`data.sku_ids`，与请求 skus 顺序一致）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/channels/api/locallife/shop/api_addlocalproduct
- 官方业务限制：①**product_type 与 code_source_type 创建后不支持修改**；②本接口不支持直接设置库存， / 创建完成后须调用库存管理接口；③请求体 `verify_at_store`=0 时必须填 `verify_page`， / 核销体验不达标将被断播处罚；④商品名称 3–60 字符，不得仅为数字或英文，不得含非法字符； / ⑤请求体 `attr_kv_map` 为字符串键值 map（值为 JSON 字符串化属性，键集参照官方 / 「代金券属性KV / 兑换券属性KV」小节）；⑥请求体 `head_imgs` 须用「上传图片」接口获取； / ⑦专属错误码 `10020021`（标题过长）/ `10020135`（use_date 跨度超限：1–180 天，开始不晚于当前+30 天）/ / `10020149`（套餐分组内商品数超出上限 20）/ `10020176`（售价不能大于原价）等。

### 6. `GetLocallifeProductListAsync` — 查询团购优惠列表（`getlocalproductlist`，不填 status 默认拉全部商品，不包含回收站）。

- **声明接口**：[IChannelsWindowService](./IChannelsWindowService.cs)
- **官方契约**：`POST /channels/ec/product/locallife/list/get`

- **请求体**：`ChannelsGetLocallifeProductListRequest` — 查询请求（全字段选填：`status` 商品状态 / `page_size` 默认 10 不超过 30 / `next_key` 翻页上下文）。
- **返回**：`ChannelsGetLocallifeProductListResponse` — 团购优惠商品 ID 列表（`product_ids`）与翻页信息（`next_key`/`total_num`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/channels/api/locallife/shop/api_getlocalproductlist
- 官方业务限制：本接口无特殊注意事项；专属错误码 `10020150`（店铺没有权限调用该接口）。

### 7. `GetLocallifeProductAsync` — 查询团购优惠详情（`getlocalproduct`，可分别获取线上数据与草稿数据）。

- **声明接口**：[IChannelsWindowService](./IChannelsWindowService.cs)
- **官方契约**：`POST /channels/ec/product/locallife/get`

- **请求体**：`ChannelsGetLocallifeProductRequest` — 查询请求（`product_id` 必填；`data_type` 选填，默认取 1：1=线上数据 2=草稿数据 3=同时获取）。
- **返回**：`ChannelsGetLocallifeProductResponse` — 商品线上数据（`online_data`，data_type=2 时不返回）与草稿数据（`draft_data`，data_type=1 时不返回；data_type=3 且从未上架过时不返回线上数据）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/channels/api/locallife/shop/api_getlocalproduct
- 官方业务限制：①上架过的商品才有线上数据；②请求体 `product_id` 为平台生成的 id； / ③专属错误码 `10020051`（参数错误）/ `10020065`（商品未上架或不存在）。
- **官方文档矛盾照录**：响应参数表把 `product_id`/`category_id`/`brand_id` 标注为 number、 / 返回示例为字符串（如 "15000000134007"）——SDK 按返回示例与同域 add/update 请求参数表 string(uint64) 建模为字符串。

### 8. `AuditFreeUpdateLocallifeProductAsync` — 免审更新团购优惠（`updatelocalauditfree`，针对已上线的团购优惠免审核更新售卖价格）。

- **声明接口**：[IChannelsWindowService](./IChannelsWindowService.cs)
- **官方契约**：`POST /channels/ec/product/locallife/auditfree`

- **请求体**：`ChannelsAuditFreeUpdateLocallifeProductRequest` — 更新请求（`product_id` 必填；`skus` 必填且更新时 sku_id 必填）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/channels/api/locallife/shop/api_updatelocalauditfree
- 官方业务限制：①仅针对**已上线**的本地团购优惠；②本接口不支持直接设置库存数量，须调用修改库存接口； / ③更新成功后将通过「团购优惠更新回调」通知商家；④专属错误码 `10020013`（sku 数量不合理）/ / `10020034`（sku_id 填写有误）/ `10020049`（商品正在审核中，无法编辑）/ `10020176`（售价不能大于原价）等。

### 9. `UpdateLocallifeProductAsync` — 更新团购优惠（`updatelocalproduct`，全量更新；更新成功后通过「团购优惠更新回调」通知商家）。

- **声明接口**：[IChannelsWindowService](./IChannelsWindowService.cs)
- **官方契约**：`POST /channels/ec/product/locallife/update`

- **请求体**：`ChannelsUpdateLocallifeProductRequest` — 更新请求（`product_id`/`product_type`/`product_name`/`category_id`/`brand_id`/`head_imgs`/`code_source_type`/`attr_kv_map`/`skus` 必填；更新时 sku_id 必填）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/channels/api/locallife/shop/api_updatelocalproduct
- 官方业务限制：①**product_type 与 code_source_type 创建后不支持修改**；②本接口不支持直接设置库存数量， / 须调用修改库存接口；③请求体 `verify_at_store`=0 时必须填 `verify_page`； / ④专属错误码 `10020049`（商品正在审核中，无法编辑或删除，请先调用撤回商品审核接口）/ / `10020148`（套餐分组数超限：兑换券上限 10 个、次卡限 1 个）/ `10020176`（售价不能大于原价）等。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Channels.DataModels/Window`](../../../Mud.Wechat.Channels.DataModels/Window/) 内共 **27** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **27** 个，`SerializerClassName` = `Window`。

AOT 源生成上下文：[`WindowJsonContext.g.cs`](./../../../Mud.Wechat.Channels.DataModels/Generated/WindowJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `ChannelsResponse` | 应答 | [ChannelsResponse.cs](../../../Mud.Wechat.Channels.DataModels/ChannelsResponse.cs) | `errcode`, `errmsg` |
| `ChannelsAddWindowProductRequest` | 请求体 | [Window/Product/ChannelsAddWindowProductRequest.cs](../../../Mud.Wechat.Channels.DataModels/Window/Product/ChannelsAddWindowProductRequest.cs) | `product_id`, `appid`, `is_hide_for_window` |
| `ChannelsGetWindowProductResponse` | 应答 | [Window/Product/ChannelsGetWindowProductResponse.cs](../../../Mud.Wechat.Channels.DataModels/Window/Product/ChannelsGetWindowProductResponse.cs) | `product`, `cps_products` |
| `ChannelsGetWindowProductRequest` | 请求体 | [Window/Product/ChannelsGetWindowProductRequest.cs](../../../Mud.Wechat.Channels.DataModels/Window/Product/ChannelsGetWindowProductRequest.cs) | `product_id`, `appid`, `need_cps_product` |
| `ChannelsGetWindowProductListResponse` | 应答 | [Window/Product/ChannelsGetWindowProductListResponse.cs](../../../Mud.Wechat.Channels.DataModels/Window/Product/ChannelsGetWindowProductListResponse.cs) | `products`, `last_buffer`, `total_num` |
| `ChannelsGetWindowProductListRequest` | 请求体 | [Window/Product/ChannelsGetWindowProductListRequest.cs](../../../Mud.Wechat.Channels.DataModels/Window/Product/ChannelsGetWindowProductListRequest.cs) | `appid`, `branch_id`, `page_size`, `page_index`, `last_buffer`, `need_total_num`, `query_cps_product` |
| `ChannelsOffWindowProductRequest` | 请求体 | [Window/Product/ChannelsOffWindowProductRequest.cs](../../../Mud.Wechat.Channels.DataModels/Window/Product/ChannelsOffWindowProductRequest.cs) | `product_id`, `appid` |
| `ChannelsAddLocallifeProductResponse` | 应答 | [Window/LocallifeProduct/ChannelsAddLocallifeProductResponse.cs](../../../Mud.Wechat.Channels.DataModels/Window/LocallifeProduct/ChannelsAddLocallifeProductResponse.cs) | `data` |
| `ChannelsAddLocallifeProductRequest` | 请求体 | [Window/LocallifeProduct/ChannelsAddLocallifeProductRequest.cs](../../../Mud.Wechat.Channels.DataModels/Window/LocallifeProduct/ChannelsAddLocallifeProductRequest.cs) | `out_product_id`, `product_type`, `product_name`, `category_id`, `brand_id`, `head_imgs`, `product_qua_infos`, `verify_page`, `verify_at_store`, `code_source_type`, `attr_kv_map`, `skus` |
| `ChannelsGetLocallifeProductListResponse` | 应答 | [Window/LocallifeProduct/ChannelsGetLocallifeProductListResponse.cs](../../../Mud.Wechat.Channels.DataModels/Window/LocallifeProduct/ChannelsGetLocallifeProductListResponse.cs) | `product_ids`, `next_key`, `total_num` |
| `ChannelsGetLocallifeProductListRequest` | 请求体 | [Window/LocallifeProduct/ChannelsGetLocallifeProductListRequest.cs](../../../Mud.Wechat.Channels.DataModels/Window/LocallifeProduct/ChannelsGetLocallifeProductListRequest.cs) | `status`, `page_size`, `next_key` |
| `ChannelsGetLocallifeProductResponse` | 应答 | [Window/LocallifeProduct/ChannelsGetLocallifeProductResponse.cs](../../../Mud.Wechat.Channels.DataModels/Window/LocallifeProduct/ChannelsGetLocallifeProductResponse.cs) | `online_data`, `draft_data` |
| `ChannelsGetLocallifeProductRequest` | 请求体 | [Window/LocallifeProduct/ChannelsGetLocallifeProductRequest.cs](../../../Mud.Wechat.Channels.DataModels/Window/LocallifeProduct/ChannelsGetLocallifeProductRequest.cs) | `product_id`, `data_type` |
| `ChannelsAuditFreeUpdateLocallifeProductRequest` | 请求体 | [Window/LocallifeProduct/ChannelsAuditFreeUpdateLocallifeProductRequest.cs](../../../Mud.Wechat.Channels.DataModels/Window/LocallifeProduct/ChannelsAuditFreeUpdateLocallifeProductRequest.cs) | `product_id`, `skus` |
| `ChannelsUpdateLocallifeProductRequest` | 请求体 | [Window/LocallifeProduct/ChannelsUpdateLocallifeProductRequest.cs](../../../Mud.Wechat.Channels.DataModels/Window/LocallifeProduct/ChannelsUpdateLocallifeProductRequest.cs) | `product_id`, `product_type`, `product_name`, `category_id`, `brand_id`, `head_imgs`, `product_qua_infos`, `verify_page`, `verify_at_store`, `code_source_type`, `attr_kv_map`, `skus` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Channels;
using Mud.Wechat.Channels.DataModels.Window;

builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddWindowApi());

var service = app.Services.GetRequiredService<IChannelsWindowService>();
var ct = CancellationToken.None;
var response = await service.AddLocallifeProductAsync(new ChannelsAddLocallifeProductRequest(), ct);
// 业务失败判定：WechatChannelsException.ThrowIfFailed(response, "/channels/ec/product/locallife/add");
// 占位值仅用于编译示意；业务值须按 IChannelsWindowService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`AddLocallifeProductAsync`）；请求体 `ChannelsAddLocallifeProductRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IChannelsWindowService](./IChannelsWindowService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`ChannelsWindowContractGuards.cs`](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsWindowContractGuards.cs)、[`ChannelsTalentContractGuards.cs`](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsTalentContractGuards.cs) |
| 守卫中的代表性测试方法 | `WindowEndpoints_ShouldMatchOfficialRoutes`、`WindowEndpoints_ShouldAllBePost_WithBodyRules`、`WindowInterface_ShouldBindChannelsToken_AndRegisterUnderWindowGroup`、`WindowDataModels_ShouldBeRegisteredInJsonContext`、`WindowDataModels_ShouldLockOfficialFieldsAndSharedDtos`、`WindowEnumConstants_ShouldLockOfficialLiteralValues`、`TalentEndpoints_ShouldMatchOfficialRoutes`、`TalentEndpoints_ShouldAllBePost_WithSingleBody_AndTypedResponses`、`TalentInterface_ShouldBindChannelsToken_AndRegisterUnderTalentGroup`、`TalentDataModels_ShouldBeRegisteredInJsonContext`、`TalentDataModels_ShouldLockOfficialFieldsAndSplitDecisions`、`TalentConstants_ShouldLockOfficialEnumValues_AndStaySplitFromLeague` … |
| 模块枚举 / 装配入口 | [`ChannelsModule.cs`](../../Extensions/ChannelsModule.cs)、[`ChannelsServiceBuilder.cs`](../../Extensions/ChannelsServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Channels.DataModels/Window`](../../../Mud.Wechat.Channels.DataModels/Window/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Channels/Mud.Wechat.Channels/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`ChannelsWindowContractGuards.cs`、`ChannelsTalentContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `Wechat.Channels.AccessToken`；接口平铺在 `Mud.Wechat.Channels`，不得声明 `IsAbstract` 父接口（CH-X2）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
