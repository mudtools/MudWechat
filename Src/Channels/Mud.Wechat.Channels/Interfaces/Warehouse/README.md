# Warehouse 域 · 区域仓库

微信小店 / 视频号（channels 生态）「区域仓库」域 SDK（11 端点：仓库创建 / 列表 / 详情 /
修改 / 覆盖区域批量增删 / 地址优先级查设 / 库存更新查询 + 地址行政编码）。 接口：[IChannelsWarehouseService](./IChannelsWarehouseService.cs)；注册组 `Warehouse`；模块枚举 `ChannelsModule.Warehouse`（装配入口见 [ChannelsServiceBuilder.cs](../../Extensions/ChannelsServiceBuilder.cs)）。逐端点核验 2026-10-18。

> **事实来源限定**：本文所有断言只来自本仓三份材料 —— 本目录接口文件的 XML 注释、本仓契约守卫（[ChannelsWarehouseContractGuards.cs](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsWarehouseContractGuards.cs)）、本域 DTO（[`Mud.Wechat.Channels.DataModels/Warehouse`](../../../Mud.Wechat.Channels.DataModels/Warehouse/)）。不复述外部官方文档正文；接口注释里没有的东西一律进文末「已知留档缺口」。

## 端点速览

接口注释自述计数：IChannelsWarehouseService = 11；按 HTTP 特性实计 **11** 支。

| # | 接口 | 方法 | HTTP 动词 | 路由 | 令牌 / 签名 | 备注 |
|---|---|---|---|---|---|---|
| 1 | `IChannelsWarehouseService` | `CreateWarehouseAsync` | POST | `/channels/ec/warehouse/create` | `AccessToken`（Query），Query `access_token` | 创建区域仓库（`createwarehouse`）。 |
| 2 | `IChannelsWarehouseService` | `GetWarehouseListAsync` | POST | `/channels/ec/warehouse/list/get` | `AccessToken`（Query），Query `access_token` | 查询区域仓库列表（`list/get`，查询结果按创建时间升序排序）。 |
| 3 | `IChannelsWarehouseService` | `GetWarehouseAsync` | POST | `/channels/ec/warehouse/get` | `AccessToken`（Query），Query `access_token` | 获取区域仓库（`get`，单个仓库详情含覆盖区域）。 |
| 4 | `IChannelsWarehouseService` | `UpdateWarehouseDetailAsync` | POST | `/channels/ec/warehouse/detail/update` | `AccessToken`（Query），Query `access_token` | 修改区域仓库详情（`detail/update`，仅仓库名称与介绍）。 |
| 5 | `IChannelsWarehouseService` | `AddCoverLocationsAsync` | POST | `/channels/ec/warehouse/coverlocations/add` | `AccessToken`（Query），Query `access_token` | 批量增加覆盖区域（`coverlocations/add`）。 |
| 6 | `IChannelsWarehouseService` | `DeleteCoverLocationsAsync` | POST | `/channels/ec/warehouse/coverlocations/del` | `AccessToken`（Query），Query `access_token` | 批量删除覆盖区域（`coverlocations/del`）。 |
| 7 | `IChannelsWarehouseService` | `GetAddressPrioritySortAsync` | POST | `/channels/ec/warehouse/address/prioritysort/get` | `AccessToken`（Query），Query `access_token` | 获取指定地址下区域仓库的优先级（`address/prioritysort/get`）。 |
| 8 | `IChannelsWarehouseService` | `SetAddressPrioritySortAsync` | POST | `/channels/ec/warehouse/address/prioritysort/set` | `AccessToken`（Query），Query `access_token` | 设置指定地址下区域仓库的优先级（`address/prioritysort/set`）。 |
| 9 | `IChannelsWarehouseService` | `UpdateWarehouseStockAsync` | POST | `/channels/ec/warehouse/stock/update` | `AccessToken`（Query），Query `access_token` | 更新区域仓库存数量（`stock/update`，增加 / 减少 / 设置三种修改类型）。 |
| 10 | `IChannelsWarehouseService` | `GetWarehouseStockAsync` | POST | `/channels/ec/warehouse/stock/get` | `AccessToken`（Query），Query `access_token` | 获取区域仓库存数量（`stock/get`）。 |
| 11 | `IChannelsWarehouseService` | `GetAddressCodeAsync` | POST | `/channels/ec/basics/addresscode/get` | `AccessToken`（Query），Query `access_token` | 获取地址行政编码（`addresscode/get`，最多获取 4 级地址的行政编码）。 |

## 令牌与认证

本域全部端点消费 `Wechat.Channels.AccessToken`，按官方契约以 **Query** 参数 `access_token` 注入（MUD005 已知接受风险：无法改用 Header）。接口上的 `[Token]` 是唯一声明点。

## 域级契约

以下为接口级 XML 注释原文摘录（`<para>` 逐段转写，仅做 Markdown 标记转换，不改语义）。

**与「本地生活门店」「质检仓」概念严格区分**：本域是微信小店**发货侧区域仓库**
（`/channels/ec/warehouse/*`，小店 `access_token`，第三方代调用权限集 id 129）；
质检送检仓为「质检管理」域（`/channels/ec/qic/*`），本地生活门店为 Locallife 域。

**路由前缀并存**：本域 10 端点走 `/channels/ec/warehouse/*`，1 端点
（`获取地址行政编码`）走 `/channels/ec/basics/addresscode/get`（官方将该端点
归入区域仓库 API 族，路由为 basics 历史前缀，照抄官方原文不得改写）。

**地址编码口径**：地址行政编码为 9 位数字（如广东省 `440000000`），来源为
`获取地址行政编码` 端点；覆盖区域按「从右往左、0 为通配」规则匹配（见各方法 remarks）。

**MUD005 已知接受风险**：微信小店官方契约强制令牌走 Query 参数（`access_token`），
无法改用 Header；库内遥测与异常消息的 URL 已由组件 `SensitiveUrlRedactor` 与
`WechatChannelsException` 构造期脱敏处置。

## 端点契约明细

逐端点原文摘录。参数说明来自 `<param>`；「备注」来自 `<remarks>`，仅将跨多支端点重复出现的同文提示去重后提升到本节开头。

### 跨端点重复提示（原文去重）

- （出现 10 次）第三方平台代调用所属权限集 id 为 `129`。
- （出现 11 次）MUD005：令牌强制走 Query 参数 `access_token`（官方契约，无法改用 Header）。

### 1. `CreateWarehouseAsync` — 创建区域仓库（`createwarehouse`）。

- **声明接口**：[IChannelsWarehouseService](./IChannelsWarehouseService.cs)
- **官方契约**：`POST /channels/ec/warehouse/create`

- **请求体**：`ChannelsCreateWarehouseRequest` — 创建请求（`out_warehouse_id`/`name`/`intro` 必填，`cover_locations` 可选）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/warehouse/api_createwarehouse
- 官方业务限制：①`out_warehouse_id` 为外部仓库 ID，命名规则为字母数字 `[0-9, a-z, A-Z]` / 加特殊字符 `[-_.]`，最大 128 字符，**一个店铺下同一个外部 ID 只能创建一个仓库**； / ②`cover_locations` 覆盖规则（A/B/C/D 均非 0）：仅填 `address_id1`=覆盖 A 省份， / 填 1、2=覆盖 A 省 B 市，填 1~3=覆盖 A 省 B 市 C 区，填 1~4=覆盖 A 省 B 市 C 区 D 街道； / ③覆盖区域可在创建后经「批量增加覆盖区域」接口添加。
- 官方错误码：`10020200`（out_warehouse_id 命名非法）/ `10020203`（address_id 非法或关联关系非法）/ `10020205`（仓库名称或仓库介绍为空）。

### 2. `GetWarehouseListAsync` — 查询区域仓库列表（`list/get`，查询结果按创建时间升序排序）。

- **声明接口**：[IChannelsWarehouseService](./IChannelsWarehouseService.cs)
- **官方契约**：`POST /channels/ec/warehouse/list/get`

- **请求体**：`ChannelsGetWarehouseListRequest` — 查询请求（`page_size` 必填且最大不超过 10；`next_key` 翻页上下文选填）。
- **返回**：`ChannelsGetWarehouseListResponse` — 外部仓库 ID 列表与翻页上下文（`out_warehouse_ids`/`next_key`，`next_key` 为空表示当前是最后一页）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/warehouse/api_getwarehouselist
- 官方业务限制：①`page_size` 最大不超过 10；②查询结果**按创建时间升序排序**； / ③翻页传上次返回的 `next_key`（不传默认取第一页），返回的 `next_key` 为空即最后一页。
- 官方错误码：无特殊错误码（走通用错误码）。

### 3. `GetWarehouseAsync` — 获取区域仓库（`get`，单个仓库详情含覆盖区域）。

- **声明接口**：[IChannelsWarehouseService](./IChannelsWarehouseService.cs)
- **官方契约**：`POST /channels/ec/warehouse/get`

- **请求体**：`ChannelsGetWarehouseRequest` — 查询请求（`out_warehouse_id` 必填，可从查询区域仓库列表接口获取）。
- **返回**：`ChannelsGetWarehouseResponse` — 仓库详情（`out_warehouse_id`/`name`/`intro`/`cover_locations`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/warehouse/api_getwarehouse
- 官方业务限制：本接口无特殊注意事项；返回的 `cover_locations` 可作为 / 「批量增加/删除覆盖区域」的入参来源。
- 官方错误码：`10020202`（out_warehouse_id 不存在）。

### 4. `UpdateWarehouseDetailAsync` — 修改区域仓库详情（`detail/update`，仅仓库名称与介绍）。

- **声明接口**：[IChannelsWarehouseService](./IChannelsWarehouseService.cs)
- **官方契约**：`POST /channels/ec/warehouse/detail/update`

- **请求体**：`ChannelsUpdateWarehouseDetailRequest` — 修改请求（`out_warehouse_id`/`name`/`intro` 均必填）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/warehouse/api_updatewarehousedetail
- 官方业务限制：本接口无特殊注意事项；覆盖区域的修改不走本接口， / 须用「批量增加覆盖区域」与「批量删除覆盖区域」两个接口。
- 官方错误码：`10020202`（out_warehouse_id 不存在）/ `10020205`（仓库名称或仓库介绍为空）。

### 5. `AddCoverLocationsAsync` — 批量增加覆盖区域（`coverlocations/add`）。

- **声明接口**：[IChannelsWarehouseService](./IChannelsWarehouseService.cs)
- **官方契约**：`POST /channels/ec/warehouse/coverlocations/add`

- **请求体**：`ChannelsWarehouseCoverLocationsRequest` — 增加请求（`out_warehouse_id`/`cover_locations` 必填；覆盖区域元素可从获取区域仓库接口获得）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/warehouse/api_addcoverlocations
- 官方业务限制：①覆盖规则（A/B/C/D 均非 0）：仅填 `address_id1`=覆盖 A 省份， / 填 1、2=覆盖 A 省 B 市，填 1~3=覆盖 A 省 B 市 C 区，填 1~4=覆盖 A 省 B 市 C 区 D 街道； / ②`address_id1` 必填，其余地址可留空（填 0 视为通配）。
- 官方错误码：`10020202`（out_warehouse_id 不存在）/ `10020203`（address_id 非法或关联关系非法）。

### 6. `DeleteCoverLocationsAsync` — 批量删除覆盖区域（`coverlocations/del`）。

- **声明接口**：[IChannelsWarehouseService](./IChannelsWarehouseService.cs)
- **官方契约**：`POST /channels/ec/warehouse/coverlocations/del`

- **请求体**：`ChannelsWarehouseCoverLocationsRequest` — 删除请求（`out_warehouse_id`/`cover_locations` 必填；与批量增加覆盖区域请求体字段集一致 ⇒ 共用请求 DTO）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/warehouse/api_delcoverlocations
- 官方业务限制：覆盖规则与「批量增加覆盖区域」一致（仅填 `address_id1`=覆盖 A 省份， / 逐级细化至 A 省 B 市 C 区 D 街道）；`address_id1` 必填，其余地址可留空。
- 官方错误码：`10020202`（out_warehouse_id 不存在）。

### 7. `GetAddressPrioritySortAsync` — 获取指定地址下区域仓库的优先级（`address/prioritysort/get`）。

- **声明接口**：[IChannelsWarehouseService](./IChannelsWarehouseService.cs)
- **官方契约**：`POST /channels/ec/warehouse/address/prioritysort/get`

- **请求体**：`ChannelsGetAddressPrioritySortRequest` — 查询请求（`address_id1` 省份编码必填，其余级别选填）。
- **返回**：`ChannelsGetAddressPrioritySortResponse` — 按优先级从高到低排序的外部仓库 ID 列表（`priority_sort`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/warehouse/api_getaddressprioritysort
- 官方业务限制：①返回的 `priority_sort` 按 `out_warehouse_id` 排列、优先级从高到低； / ②未通过「设置指定地址下区域仓库的优先级」设置时，默认按区域仓库创建时间作为优先级 / （越早创建优先级越高）；③下单匹配原则按照地址从右往左进行匹配（详见官方「更新区域仓库存数量」接口）。
- 官方错误码：无特殊错误码（走通用错误码）。

### 8. `SetAddressPrioritySortAsync` — 设置指定地址下区域仓库的优先级（`address/prioritysort/set`）。

- **声明接口**：[IChannelsWarehouseService](./IChannelsWarehouseService.cs)
- **官方契约**：`POST /channels/ec/warehouse/address/prioritysort/set`

- **请求体**：`ChannelsSetAddressPrioritySortRequest` — 设置请求（`address_id1` 必填；`priority_sort` 为按优先级从高到低排列的外部仓库 ID 列表）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/warehouse/api_setaddressprioritysort
- 官方业务限制：①若未设置优先级，默认按区域仓库创建时间作为优先级（越早创建优先级越高）； / ②下单匹配原则按照地址从右往左进行匹配；③`priority_sort` 列表内 `out_warehouse_id` 不得重复。
- 官方错误码：`10020202`（out_warehouse_id 不存在）/ `10020203`（address_id 非法或关联关系非法）/ `10020206`（重复的 out_warehouse_id）。

### 9. `UpdateWarehouseStockAsync` — 更新区域仓库存数量（`stock/update`，增加 / 减少 / 设置三种修改类型）。

- **声明接口**：[IChannelsWarehouseService](./IChannelsWarehouseService.cs)
- **官方契约**：`POST /channels/ec/warehouse/stock/update`

- **请求体**：`ChannelsUpdateWarehouseStockRequest` — 更新请求（`product_id`/`sku_id`/`out_warehouse_id`/`diff_type`/`num` 均必填）。
- **返回**：`ChannelsResponse` — 仅 `errcode`/`errmsg`（故返回 `ChannelsResponse`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/warehouse/api_updatewarehousestock
- 官方业务限制：①修改类型枚举见 `ChannelsWarehouseStockDiffTypes`（1 增加 / 2 减少 / 3 设置）； / ②官方下单匹配原则：**按照地址从右往左进行匹配**——先筛选库存 ≥ 用户下单数量的区域仓， / 再按地址从右往左逐级匹配（通配符 0 可匹配任意地址，精准匹配优先于通配）， / 最后取符合条件集合中优先级最高的区域仓。
- 官方错误码：`10020052`（商品不存在）/ `10020202`（out_warehouse_id 不存在）/ `10020204`（该 sku 设置区域仓数量超过限制）。

### 10. `GetWarehouseStockAsync` — 获取区域仓库存数量（`stock/get`）。

- **声明接口**：[IChannelsWarehouseService](./IChannelsWarehouseService.cs)
- **官方契约**：`POST /channels/ec/warehouse/stock/get`

- **请求体**：`ChannelsGetWarehouseStockRequest` — 查询请求（`product_id`/`sku_id`/`out_warehouse_id` 均必填）。
- **返回**：`ChannelsGetWarehouseStockResponse` — 库存数量（`num`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/warehouse/api_getwarehousestock
- 官方业务限制：本接口无特殊注意事项；库存值随「更新区域仓库存数量」接口维护。
- 官方错误码：`10020052`（商品不存在）。

### 11. `GetAddressCodeAsync` — 获取地址行政编码（`addresscode/get`，最多获取 4 级地址的行政编码）。

- **声明接口**：[IChannelsWarehouseService](./IChannelsWarehouseService.cs)
- **官方契约**：`POST /channels/ec/basics/addresscode/get`

- **请求体**：`ChannelsGetAddressCodeRequest` — 查询请求（`addr_code` 选填——不填或填 0 时获取全国的省级行政编码）。
- **返回**：`ChannelsGetAddressCodeResponse` — 本行政编码地址信息（`addrs_msg`）与下一级所有地址信息（`next_level_addrs`）。

**备注（原文）**

- 官方文档：https://developers.weixin.qq.com/doc/store/shop/API/warehouse/api_getaddresscode
- 官方业务限制：①最多获取 **4 级地址**的行政编码（省 / 市 / 区县 / 街道， / 地址级别枚举见 `ChannelsAddressLevels`）；②`addr_code` 不填或填 0 时 / 返回全国省级行政编码（官方返回示例中根节点「中华人民共和国」`level`=`0`，照录）； / ③地址编码可逐级下钻，作为「创建区域仓库」等接口 `address_id1~4` 的取值来源。
- 官方错误码：`10021402`（无效的地址编码）。
- 第三方平台代调用所属权限集 id 为 `129`、`131`、`192`；官方适用范围同时放行「微信小店」与「小店供货商」。

## DTO 与序列化面

本域 DTO 目录 [`Mud.Wechat.Channels.DataModels/Warehouse`](../../../Mud.Wechat.Channels.DataModels/Warehouse/) 内共 **21** 个 `public class/record/enum/struct` 声明；其中 `[HttpJsonSerializable]` 标注 **21** 个，`SerializerClassName` = `Warehouse`。

AOT 源生成上下文：[`WarehouseJsonContext.g.cs`](./../../../Mud.Wechat.Channels.DataModels/Generated/WarehouseJsonContext.g.cs)（生成物，勿手改）。

| 端点引用的类型 | 角色 | 定义位置 | JSON 字段（前缀） |
|---|---|---|---|
| `ChannelsResponse` | 应答 | [ChannelsResponse.cs](../../../Mud.Wechat.Channels.DataModels/ChannelsResponse.cs) | `errcode`, `errmsg` |
| `ChannelsCreateWarehouseRequest` | 请求体 | [Warehouse/ChannelsCreateWarehouseRequest.cs](../../../Mud.Wechat.Channels.DataModels/Warehouse/ChannelsCreateWarehouseRequest.cs) | `out_warehouse_id`, `name`, `intro`, `cover_locations` |
| `ChannelsGetWarehouseListResponse` | 应答 | [Warehouse/ChannelsGetWarehouseListResponse.cs](../../../Mud.Wechat.Channels.DataModels/Warehouse/ChannelsGetWarehouseListResponse.cs) | `data` |
| `ChannelsGetWarehouseListRequest` | 请求体 | [Warehouse/ChannelsGetWarehouseListRequest.cs](../../../Mud.Wechat.Channels.DataModels/Warehouse/ChannelsGetWarehouseListRequest.cs) | `page_size`, `next_key` |
| `ChannelsGetWarehouseResponse` | 应答 | [Warehouse/ChannelsGetWarehouseResponse.cs](../../../Mud.Wechat.Channels.DataModels/Warehouse/ChannelsGetWarehouseResponse.cs) | `data` |
| `ChannelsGetWarehouseRequest` | 请求体 | [Warehouse/ChannelsGetWarehouseRequest.cs](../../../Mud.Wechat.Channels.DataModels/Warehouse/ChannelsGetWarehouseRequest.cs) | `out_warehouse_id` |
| `ChannelsUpdateWarehouseDetailRequest` | 请求体 | [Warehouse/ChannelsUpdateWarehouseDetailRequest.cs](../../../Mud.Wechat.Channels.DataModels/Warehouse/ChannelsUpdateWarehouseDetailRequest.cs) | `out_warehouse_id`, `name`, `intro` |
| `ChannelsWarehouseCoverLocationsRequest` | 请求体 | [Warehouse/ChannelsWarehouseCoverLocationsRequest.cs](../../../Mud.Wechat.Channels.DataModels/Warehouse/ChannelsWarehouseCoverLocationsRequest.cs) | `out_warehouse_id`, `cover_locations` |
| `ChannelsGetAddressPrioritySortResponse` | 应答 | [Warehouse/ChannelsGetAddressPrioritySortResponse.cs](../../../Mud.Wechat.Channels.DataModels/Warehouse/ChannelsGetAddressPrioritySortResponse.cs) | `data` |
| `ChannelsGetAddressPrioritySortRequest` | 请求体 | [Warehouse/ChannelsGetAddressPrioritySortRequest.cs](../../../Mud.Wechat.Channels.DataModels/Warehouse/ChannelsGetAddressPrioritySortRequest.cs) | `address_id1`, `address_id2`, `address_id3`, `address_id4` |
| `ChannelsSetAddressPrioritySortRequest` | 请求体 | [Warehouse/ChannelsSetAddressPrioritySortRequest.cs](../../../Mud.Wechat.Channels.DataModels/Warehouse/ChannelsSetAddressPrioritySortRequest.cs) | `address_id1`, `address_id2`, `address_id3`, `address_id4`, `priority_sort` |
| `ChannelsUpdateWarehouseStockRequest` | 请求体 | [Warehouse/ChannelsUpdateWarehouseStockRequest.cs](../../../Mud.Wechat.Channels.DataModels/Warehouse/ChannelsUpdateWarehouseStockRequest.cs) | `product_id`, `sku_id`, `out_warehouse_id`, `diff_type`, `num` |
| `ChannelsGetWarehouseStockResponse` | 应答 | [Warehouse/ChannelsGetWarehouseStockResponse.cs](../../../Mud.Wechat.Channels.DataModels/Warehouse/ChannelsGetWarehouseStockResponse.cs) | `data` |
| `ChannelsGetWarehouseStockRequest` | 请求体 | [Warehouse/ChannelsGetWarehouseStockRequest.cs](../../../Mud.Wechat.Channels.DataModels/Warehouse/ChannelsGetWarehouseStockRequest.cs) | `product_id`, `sku_id`, `out_warehouse_id` |
| `ChannelsGetAddressCodeResponse` | 应答 | [Warehouse/ChannelsGetAddressCodeResponse.cs](../../../Mud.Wechat.Channels.DataModels/Warehouse/ChannelsGetAddressCodeResponse.cs) | `addrs_msg`, `next_level_addrs` |
| `ChannelsGetAddressCodeRequest` | 请求体 | [Warehouse/ChannelsGetAddressCodeRequest.cs](../../../Mud.Wechat.Channels.DataModels/Warehouse/ChannelsGetAddressCodeRequest.cs) | `addr_code` |

> DTO 一律以源文件为准；本表只列端点签名**直接引用**的一层类型，嵌套类型不再递归展开。所有 DTO 不使用 `required`（仓内约束），必填性由接口注释与官方页表达，SDK 不做本地必填校验。

## 快速开始

```csharp
using Mud.Wechat.Channels;
using Mud.Wechat.Channels.DataModels.Warehouse;
using Mud.Wechat.Channels.DataModels;

builder.Services.AddChannelsApp(builder.Configuration, "ChannelsApps")
    .AddWechatChannelsApi(b => b.AddWarehouseApi());

var service = app.Services.GetRequiredService<IChannelsWarehouseService>();
var ct = CancellationToken.None;
var response = await service.AddCoverLocationsAsync(new ChannelsWarehouseCoverLocationsRequest(), ct);
// 业务失败判定：WechatChannelsException.ThrowIfFailed(response, "/channels/ec/warehouse/coverlocations/add");
// 占位值仅用于编译示意；业务值须按 IChannelsWarehouseService.cs 的 XML 注释与官方约束填写。
```

本示例选的是形参最少的一支（`AddCoverLocationsAsync`）；请求体 `ChannelsWarehouseCoverLocationsRequest` 的字段口径见「DTO 与序列化面」与接口注释，示例未逐字段赋值。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| 端点级频率限制 | 本仓接口 XML 注释**未记录**端点级 QPS / 次数数字 ⇒ **未逐项核验** | 需要限流参数时先逐页核验官方页，再补接口注释与本文 |
| 官方文档正文 | 本文只摘录本仓 XML 注释，**不复述官方页正文** | 逐参数最终口径以接口注释中的官方链接为准 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 接口声明、路由、动词、参数位置、令牌声明 | [IChannelsWarehouseService](./IChannelsWarehouseService.cs) |
| 逐参数官方说明、限制、错误码 | 上述接口文件的 XML 注释（`<summary>` / `<param>` / `<returns>` / `<remarks>`） |
| 契约守卫（路由 / 计数 / DTO 登记断言） | [`ChannelsWarehouseContractGuards.cs`](../../../../../Tests/Mud.Wechat.Channels.Tests/ContractGuards/ChannelsWarehouseContractGuards.cs) |
| 守卫中的代表性测试方法 | `WarehouseEndpoints_ShouldMatchOfficialRoutes`、`WarehouseEndpoints_ShouldAllBePost_WithBodyRules`、`WarehouseInterface_ShouldBindChannelsToken_AndRegisterUnderWarehouseGroup`、`WarehouseDataModels_ShouldBeRegisteredInJsonContext`、`WarehouseDataModels_ShouldLockOfficialFieldsAndSharedDtos`、`WarehouseEnumConstants_ShouldLockOfficialLiteralValues` |
| 模块枚举 / 装配入口 | [`ChannelsModule.cs`](../../Extensions/ChannelsModule.cs)、[`ChannelsServiceBuilder.cs`](../../Extensions/ChannelsServiceBuilder.cs) |
| DTO 与 JSON 上下文 | [`Mud.Wechat.Channels.DataModels/Warehouse`](../../../Mud.Wechat.Channels.DataModels/Warehouse/) |
| 跨线基线 | [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs)（各线分节；本线口径以守卫实际断言为准） |
| 本线包级公开面与已踩陷阱 | [`Src/Channels/Mud.Wechat.Channels/README.md`](../../README.md) |
| 跨线通用约束（门禁 / AOT / 落位） | [`AGENTS.md`](../../../../../AGENTS.md) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档链接与限制（格式照既有端点，**不得弱化**），**同批**更新本域守卫（`ChannelsWarehouseContractGuards.cs`）与 [`Tests/ContractBaseline.cs`](../../../../../Tests/ContractBaseline.cs) 的跨线计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（`SerializerClassName` 取本域命名空间段）→ 跑 [`scripts/AddHttpJsonSerializable.ps1`](../../../../../scripts/AddHttpJsonSerializable.ps1) + [`scripts/GenerateJsonContext.ps1`](../../../../../scripts/GenerateJsonContext.ps1) → 更新守卫中的上下文登记断言。
3. **不要**新增令牌类型：本线复用 `Wechat.Channels.AccessToken`；接口平铺在 `Mud.Wechat.Channels`，不得声明 `IsAbstract` 父接口（CH-X2）。
4. 最后跑 [`scripts/verify-build.ps1`](../../../../../scripts/verify-build.ps1) 与 [`scripts/audit-config-keys.ps1`](../../../../../scripts/audit-config-keys.ps1)，全绿才算完成。
