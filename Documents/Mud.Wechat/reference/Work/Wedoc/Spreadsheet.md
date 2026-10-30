# 表格内容 · 工作表属性、区域读写与批量编辑

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Wedoc` 域 · 管理表格内容族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkWedocSpreadsheetService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/Spreadsheet/IWechatWorkWedocSpreadsheetService.cs)（父，承载全部 3 支）＋ 自建 [IWechatWorkWedocSpreadsheetService_Internal.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/Spreadsheet/IWechatWorkWedocSpreadsheetService_Internal.cs) / 第三方 [_ThirdParty.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/Spreadsheet/IWechatWorkWedocSpreadsheetService_ThirdParty.cs) / 代开发 [_Provider.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/Spreadsheet/IWechatWorkWedocSpreadsheetService_Provider.cs)（三个零端点子接口） |
| DTO 源文件 | [DataModels/Wedoc/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Wedoc/) 与 [RequestModel/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Wedoc/RequestModel/)（本族 3 个请求/响应 + 操作体与单元格数据 13 型，`SerializerClassName = Wedoc` → `WedocJsonContext`） |
| 契约守卫 | [WechatWedocContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatWedocContractGuards.cs)（WD3 本族 3 路由 / 九族 69 端点总表 / 层级与令牌 / DTO 292 型登记）＋ G5 ＋ TO1~TO3 ＋ N1~N3 |
| 官方文档 | 编辑表格内容 [101168](https://developer.work.weixin.qq.com/document/path/101168) / [101190](https://developer.work.weixin.qq.com/document/path/101190) / [101169](https://developer.work.weixin.qq.com/document/path/101169)；获取表格数据 [97711](https://developer.work.weixin.qq.com/document/path/97711) / [98031](https://developer.work.weixin.qq.com/document/path/98031) / [98038](https://developer.work.weixin.qq.com/document/path/98038)；获取表格行列信息 [97661](https://developer.work.weixin.qq.com/document/path/97661) / [98030](https://developer.work.weixin.qq.com/document/path/98030) / [98037](https://developer.work.weixin.qq.com/document/path/98037)（顺序为自建 / 第三方 / 代开发） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：WD3 路由表 + 3 支端点 XML 注释 + 操作体/单元格 DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/README.md) 逐端点核验（2026-10-18）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

`doc_type = 4` 的**在线表格**（Excel 形态）内容面只有三支：**查有哪些工作表 → 读某块区域 → 批量改**。

| 端点 | 用途 | 读/写 |
|---|---|---|
| E1 `GetSpreadsheetPropertiesAsync` | 取工作表列表（`sheet_id` / 名称 / 总行数 / 总列数） | 读 |
| E2 `GetSpreadsheetDataAsync` | 按 A1 区域读单元格数据 | 读 |
| E3 `BatchUpdateSpreadsheetAsync` | 一次 ≤5 个操作：加表 / 删表 / 写区域 / 删行列 | 写 |

**调用顺序是固定的**：E1 拿 `sheet_id` → E2 用 `sheet_id` + A1 区域读 → E3 用 `sheet_id` 写；`sheet_id` **只能从 E1 或 E3 的加表响应里取**，不能凭名称猜。

与其它族的分界：本族只服务 `doc_type = 4`。文档是 [Document.md](./Document.md)、智能表格是 [SmartSheet.md](./SmartSheet.md)、智能文档是 [SmartDoc.md](./SmartDoc.md)；表格的建/删/改名在 [Wedoc.md](./Wedoc.md)。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | `WechatAppType.Internal` / `ThirdParty` / `Provider` | 注册期 `AddWechatApp` |
| 必填配置属性 | 自建：`CorpId` + `AgentId` + `AgentSecret`；第三方 / 代开发：`CorpId` + `ProviderSecret` + `SuiteId` + `SuiteSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → Query `access_token` | — |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal`；第三方 / 代开发 `Wechat.AccessToken@Corp` | 运行期归属校验 |
| 企业作用域 | 第三方 / 代开发必须先 `UseCorpScope(...)` | 调用前 |

### 2.2 权限与业务前置

- 自建应用须用配置到「可调用应用」列表的 secret 换取的 `access_token`；第三方 / 代开发须具有「**文档**」权限；只能操作**应用自己创建**的表格。
- `docid` 来自 [Wedoc.md](./Wedoc.md) E1 的创建响应（`doc_type = 4`），需已持久化。
- **规模闸**：读/写区域**行 ≤1000、列 ≤200、单元格总数 ≤10000**；新增工作表**列数 ≤200、单元格 ≤10000**。
- **A1 表示法**：`A1:A1` 单格、`A1:B5` 区域；**左上角必须在右下角左上方**，`B5:A1` 非法。
- **批量语义与文档族不同**：单次 `requests` **≤5**，**逐个顺序执行、遇错即止**——错误发生前的操作**已经生效**（不是 [Document.md](./Document.md) 的全或无）。
- **索引基数不一致**：`grid_data.start_row` / `start_column` **从 0 开始**；`delete_dimension_request.start_index` / `end_index` **从 1 开始**且为左闭右开区间。
- **数据安全**：单元格内容属业务数据；日志只记 `sheet_id`、区域、更新单元格数，不打印单元格值。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 | 官方文档页（自建 / 第三方 / 代开发） |
|---|---|---|---|---|
| E1 获取表格行列信息 | ✅ | ✅ | ✅ | 97661 / 98030 / 98037 |
| E2 获取表格数据 | ✅ | ✅ | ✅ | 97711 / 98031 / 98038 |
| E3 编辑表格内容 | ✅ | ✅ | ✅ | 101168 / 101190 / 101169 |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkWedocSpreadsheetService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **3** | 否 |
| `IWechatWorkInternalWedocSpreadsheetService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyWedocSpreadsheetService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderWedocSpreadsheetService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `Wedoc`、装配入口 `AddWedocApi()`；子接口 `InheritedFrom = nameof(WechatWorkWedocSpreadsheetService)`。

## 4. 端点

### E1. `GetSpreadsheetPropertiesAsync` — 获取表格行列信息

```csharp
[Post("/cgi-bin/wedoc/spreadsheet/get_sheet_properties")]
Task<GetWedocSpreadsheetPropertiesResponse> GetSpreadsheetPropertiesAsync(
    [Body] GetWedocSpreadsheetPropertiesRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/wedoc/spreadsheet/get_sheet_properties` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`GetWedocSpreadsheetPropertiesRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Docid` | `docid` | `string?` | 是 | 在线表格 id（`doc_type = 4`） |

**响应参数**（`GetWedocSpreadsheetPropertiesResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `properties` | `Properties` | `List<WedocSpreadsheetProperties>?` | 工作表属性列表 |

`WedocSpreadsheetProperties` 元素：

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `sheet_id` | `SheetId` | `string?` | 工作表唯一标识；E2 / E3 的定位键 |
| `title` | `Title` | `string?` | 工作表名称 |
| `row_count` | `RowCount` | `long?` | 总行数 |
| `column_count` | `ColumnCount` | `long?` | 总列数 |

**本端点注意事项**

- `sheet_id` 是 E2 / E3 的必填参数，**只能从这里取**（或 E3 加表响应的 `properties.sheet_id`）；官方无「按名称查 `sheet_id`」的独立端点。
- 返回的是**全部工作表**的元数据，不含单元格值；读内容需要再用 E2 按区域拉。
- `row_count` / `column_count` 是边界：A1 区域越界会失败，拼 `range` 前先按这两个值收敛。

### E2. `GetSpreadsheetDataAsync` — 获取表格数据

```csharp
[Post("/cgi-bin/wedoc/spreadsheet/get_sheet_range_data")]
Task<GetWedocSpreadsheetDataResponse> GetSpreadsheetDataAsync(
    [Body] GetWedocSpreadsheetDataRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/wedoc/spreadsheet/get_sheet_range_data`（读接口官方也是 POST） |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`GetWedocSpreadsheetDataRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Docid` | `docid` | `string?` | 是 | 在线表格 id |
| `SheetId` | `sheet_id` | `string?` | 是 | 工作表 id（来自 E1） |
| `Range` | `range` | `string?` | 是 | A1 表示法：`A1:A1` 单格、`A1:B5` 区域；左上角必须位于右下角左上方；行 ≤1000、列 ≤200、单元格 ≤10000 |

**响应参数**（`GetWedocSpreadsheetDataResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `data` | `Data` | `WedocSpreadsheetData?` | 结果容器 |
| `data.result` | `Data.Result` | `WedocGridData?` | 表格数据网格 |

`WedocGridData` / `WedocRowData` / `WedocCellData` / `WedocCellValue` 元素：

| 层级 | 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|---|
| GridData | `start_row` | `StartRow` | `long?` | 起始行，**从 0 开始** |
| GridData | `start_column` | `StartColumn` | `long?` | 起始列，**从 0 开始** |
| GridData | `rows` | `Rows` | `List<WedocRowData>?` | 各行数据 |
| RowData | `values` | `Values` | `List<WedocCellData>?` | 行内各单元格 |
| CellData | `cell_value` | `CellValue` | `WedocCellValue?` | 值：`text` 文本 / `link`（`url` + `text`）二选一 |
| CellData | `cell_format` | `CellFormat` | `WedocCellFormat?` | 样式：`text_format`（`font` / `font_size` ≤72 / `bold` / `italic` / `strikethrough` / `underline` / `color`） |

**本端点注意事项**

- 空单元格在 `values` 中可能表现为空对象，不要用「数组长度」推断有值单元格数。
- 读取是**按区域**的：跨表要分多次调用，一次只能读一个 `sheet_id`。
- 值类型目前只支持**文本与链接**（`cell_value` 二选一），公式等未建模。

### E3. `BatchUpdateSpreadsheetAsync` — 编辑表格内容

```csharp
[Post("/cgi-bin/wedoc/spreadsheet/batch_update")]
Task<BatchUpdateWedocSpreadsheetResponse> BatchUpdateSpreadsheetAsync(
    [Body] BatchUpdateWedocSpreadsheetRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `POST /cgi-bin/wedoc/spreadsheet/batch_update` |
| 凭据 | Query `access_token` |
| 适用应用类型 | 自建 / 第三方 / 代开发 |

**请求参数**（`BatchUpdateWedocSpreadsheetRequest`）

| 参数 | JSON 字段名 | 类型（C#） | 官方必填 | 说明与限制 |
|---|---|---|---|---|
| `Docid` | `docid` | `string?` | 是 | 在线表格 id |
| `Requests` | `requests` | `List<WedocSpreadsheetUpdateOperation>?` | 是 | 操作列表，**≤5 个**；顺序执行、**遇错即止**（前面的已生效） |

**操作体**（`WedocSpreadsheetUpdateOperation`）：四个字段**互斥**，一个对象只填一个

| JSON 键 | 属性 | 请求子结构 | 关键字段 |
|---|---|---|---|
| `add_sheet_request` | `AddSheetRequest` | `WedocSpreadsheetAddSheetRequest` | `title`；`row_count`；`column_count`（列 ≤200、单元格 ≤10000） |
| `delete_sheet_request` | `DeleteSheetRequest` | `WedocSpreadsheetDeleteSheetRequest` | `sheet_id` |
| `update_range_request` | `UpdateRangeRequest` | `WedocSpreadsheetUpdateRangeRequest` | `sheet_id` + `grid_data`（行 ≤1000、列 ≤200、单元格 ≤10000） |
| `delete_dimension_request` | `DeleteDimensionRequest` | `WedocSpreadsheetDeleteDimensionRequest` | `sheet_id`；`dimension` = `ROW` / `COLUMN`；`start_index` / `end_index`（**从 1 开始**，左闭右开） |

写入数据 `WedocGridData`：`start_row` / `start_column`（**从 0 开始**）+ `rows[].values[].cell_value`。

**响应参数**（`BatchUpdateWedocSpreadsheetResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `data.responses` | `Data.Responses` | `List<WedocSpreadsheetUpdateResponse>?` | 与请求操作**一一对应**；每个元素按操作类型只承载一组响应 |
| `…add_sheet_response.properties` | `AddSheetResponse.Properties` | `WedocSpreadsheetProperties?` | 新表 `sheet_id` / 名称 / 行列数 |
| `…delete_sheet_response.sheet_id` | `DeleteSheetResponse.SheetId` | `string?` | 被删表 id |
| `…update_range_response.updated_cells` | `UpdateRangeResponse.UpdatedCells` | `long?` | 成功更新的单元格数 |
| `…delete_dimension_response.deleted` | `DeleteDimensionResponse.Deleted` | `long?` | 删除的行数或列数 |

**本端点注意事项**

- **不是全或无**：遇错即止，**之前的操作已生效**。重试前必须先读回当前状态，否则会重复插入/重复删除。
- `responses` 与 `requests` 一一对应，但**出错时可能短于请求**（后续操作未执行）——不要按下标盲取。
- `delete_dimension` 的区间是**左闭右开且从 1 开始**：删第 1 行写 `start_index = 1`、`end_index = 2`；`end_index <= start_index` 会报错，接口 remarks 明确留档「该操作会导致表格**缩表**」。
- `grid_data.start_row` / `start_column` 是 **0 基**，与 `delete_dimension` 的 **1 基**不是一套坐标，混用会错位一行一列。
- 新增工作表的 `column_count` 超 200 或单元格数超 10000 会失败；官方没有「整表替换」操作，只能按区域写。

**调用示例**（列工作表 → 读区域 → 写区域 → 加表）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Wedoc;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "wedoc-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddWedocApi());

// 第三方 / 代开发改用 IWechatWorkThirdPartyWedocSpreadsheetService / IWechatWorkProviderWedocSpreadsheetService
// 并在调用前 switcher.UseCorpScope(appKey, authCorpId, permanentCode)。
var sheet = provider.GetRequiredService<IWechatWorkInternalWedocSpreadsheetService>();
string docId = "DOCID-FAKE-PLACEHOLDER";       // 创建表格（doc_type = 4）后持久化的 docid

// ① 列工作表：sheet_id 只能从这里取
var props = await sheet.GetSpreadsheetPropertiesAsync(
    new GetWedocSpreadsheetPropertiesRequest { Docid = docId }, cancellationToken);
WechatWorkException.ThrowIfFailed(props, "/cgi-bin/wedoc/spreadsheet/get_sheet_properties");
var first = props.Properties?.FirstOrDefault() ?? throw new InvalidOperationException("表格没有工作表");
string sheetId = first.SheetId ?? throw new InvalidOperationException("工作表缺少 sheet_id");

// ② 读区域：A1 表示法，左上角必须在右下角左上方；行 ≤1000、列 ≤200、单元格 ≤10000
var range = await sheet.GetSpreadsheetDataAsync(
    new GetWedocSpreadsheetDataRequest { Docid = docId, SheetId = sheetId, Range = "A1:D10" },
    cancellationToken);
WechatWorkException.ThrowIfFailed(range, "/cgi-bin/wedoc/spreadsheet/get_sheet_range_data");
int rowCount = range.Data?.Result?.Rows?.Count ?? 0;
logger.LogInformation("工作表 {SheetId} 读到 {Rows} 行", sheetId, rowCount);

// ③ 批量写：≤5 个操作、顺序执行遇错即止；grid_data 的 start_row/start_column 从 0 开始
var updated = await sheet.BatchUpdateSpreadsheetAsync(
    new BatchUpdateWedocSpreadsheetRequest
    {
        Docid = docId,
        Requests =
        [
            new WedocSpreadsheetUpdateOperation
            {
                UpdateRangeRequest = new WedocSpreadsheetUpdateRangeRequest
                {
                    SheetId = sheetId,
                    GridData = new WedocGridData
                    {
                        StartRow = 0,
                        StartColumn = 0,
                        Rows =
                        [
                            new WedocRowData
                            {
                                Values =
                                [
                                    new WedocCellData { CellValue = new WedocCellValue { Text = "项目" } },
                                    new WedocCellData { CellValue = new WedocCellValue { Text = "状态" } },
                                ],
                            },
                        ],
                    },
                },
            },
            new WedocSpreadsheetUpdateOperation
            {
                AddSheetRequest = new WedocSpreadsheetAddSheetRequest
                {
                    Title = "归档",
                    RowCount = 100,
                    ColumnCount = 10,
                },
            },
        ],
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(updated, "/cgi-bin/wedoc/spreadsheet/batch_update");

// ④ responses 与请求一一对应，但遇错即止时可能更短：按下标取之前先比长度
var responses = updated.Data?.Responses ?? new List<WedocSpreadsheetUpdateResponse>();
logger.LogInformation("批量操作 {Requested} 个，返回 {Returned} 个结果", 2, responses.Count);
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| 遇错即止 vs 全或无 | 表格 ≤5 操作、**前面的已生效**（文档族才是全或无） | 直接重试 ⇒ 重复写入 |
| `responses` 长度 | 出错时短于 `requests` | 按下标盲取 ⇒ 越界或错配 |
| 0 基 vs 1 基 | `grid_data` 的 `start_row`/`start_column` 从 0；`delete_dimension` 的 `start_index`/`end_index` 从 1 | 错删一行一列 |
| 删除区间语义 | 左闭右开，`end_index <= start_index` 报错并**缩表** | 少删/多删且不可逆 |
| A1 方向 | `B5:A1` 非法 | 直接失败 |
| `sheet_id` 来源 | 只能从 E1 或加表响应取 | 凭名称猜 ⇒ 失败 |
| 区域规模 | 行 ≤1000、列 ≤200、单元格 ≤10000 | 超限失败（且前序操作已生效） |
| 加表列宽 | `column_count` ≤200、单元格 ≤10000 | 超限失败 |
| 值类型 | `cell_value` 只支持 `text` / `link` 二选一 | 塞两个字段 ⇒ 只有一个生效 |
| 表格 vs 文档 | 本族只服务 `doc_type = 4` | 类型错配以 `640054` 类业务码暴露 |
| 单元格内容安全 | 属业务数据 | 打印单元格 ⇒ 内容泄露进日志 |

SDK 侧通用约束：不自动判错、不做本地区域规模与互斥校验、不自动重试。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 请求成功 | 判 `IsSuccess`；批量还需看 `responses` 长度 |
| 640017 / 640018 / 640008 | 域 README 留档 | 未开启 API / 未授编辑权限 / 无管理权限 | 权限前置 |
| 640054 | 域 README 留档 | `docid` 不是目标类型 | 核对 `doc_type` |
| 40014 / 42001 / 42007 | 令牌失效码 | 应用令牌类错误 | 统一失效码处理 |
| 42011 | `InvalidProviderAccessToken` | 服务商令牌不合法 / 过期 | 失效码集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈，装配缺陷 |

A1 越界、区域超限、`sheet_id` 不存在、`end_index <= start_index` 的具体业务码以官方页为准（仓内未逐项核验）。其余见[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454)（90454）。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 批量出错时的 `responses` 形态 | 遇错即止后返回的列表是否含出错项 ⇒ 未核验 | 官方 101168 |
| 区域越界错误码 | `range` 超出 `row_count`/`column_count` 的码值 ⇒ 未留档 | 官方 97711 + 90455 |
| 空单元格表示 | 空值是否省略 `values` 元素 ⇒ 未核验 | 官方 97711 |
| 公式与数值类型 | 官方是否支持公式、数字、日期 ⇒ 本仓只建模 text/link | 官方 97711 |
| 单元格样式可写面 | `cell_format` 是否可写（本仓建模为读写共用） | 官方 101168 |
| 工作表数量上限 | 单表格最多几个工作表 ⇒ 未核验 | 官方 101168 |
| 本族频率限制 | 官方未给独立数值频率 ⇒ 走全局 | 90454 |
| 真账号验证 | 本批仅内部证据核验 | — |

## 8. 延伸阅读

- 同域兄弟：[Wedoc.md](./Wedoc.md)（建/删/改名）· [Document.md](./Document.md)（文档内容）· [SmartSheet.md](./SmartSheet.md)（智能表格）· [SmartDoc.md](./SmartDoc.md)（智能文档）
- 权限：[DocPermission.md](./DocPermission.md)
- 域总览：[Wedoc README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/README.md)
- 官方页：[编辑表格内容 101168](https://developer.work.weixin.qq.com/document/path/101168) · [获取表格数据 97711](https://developer.work.weixin.qq.com/document/path/97711) · [获取表格行列信息 97661](https://developer.work.weixin.qq.com/document/path/97661)
