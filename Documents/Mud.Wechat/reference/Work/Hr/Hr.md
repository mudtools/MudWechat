# 人事助手 · 员工花名册

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Hr` 域 · 员工花名册族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkHrService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Hr/IWechatWorkHrService.cs)（父，**零端点**）· [IWechatWorkInternalHrService](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Hr/IWechatWorkHrService_Internal.cs)（承载全部 3 支） |
| DTO 源文件 | [DataModels/Hr/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Hr/)（14 型，`SerializerClassName = Hr`，登记断言见 HR5 → `HrJsonContext`） |
| 契约守卫 | [WechatHrContractGuards](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatHrContractGuards.cs)（HR1 路由 / HR2 `fieldid` 拼写锁定 / HR3 零端点父接口 + 仅自建子接口 / HR4 令牌绑定 / HR5 JSON 上下文）＋ G5 ＋ TO1~TO3 |
| 官方文档 | 字段配置 [99131](https://developer.work.weixin.qq.com/document/path/99131) · 读取花名册 [99132](https://developer.work.weixin.qq.com/document/path/99132) · 更新花名册 [99133](https://developer.work.weixin.qq.com/document/path/99133)（均企业自建应用） |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据为守卫 + 接口/DTO XML 注释逐项比对；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（net8.0）与本批 9 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 9 段、全仓累计 111 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：HR1~HR5 + 接口与 DTO XML 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Hr/README.md)· 官方：本批未重新取文，正文口径继承接口注释留档 |

## 1. 这个接口族做什么

人事助手花名册的**配置读取 + 逐字段读写**，3 支端点构成「先看有哪些字段 → 再按字段读 → 再按字段写」的闭环：

| 端点 | 用途 |
|---|---|
| E1 `get_fields` | **GET、无业务参数**，取字段组配置（字段 id / 名称 / 类型 / 值类型 / 是否必填 / 选项枚举） |
| E2 `get_staff_info` | 按 `userid` 读花名册：`get_all` 全量，或 `fieldids` 指定字段 |
| E3 `update_staff_info` | 按 `userid` 写花名册：更新 / 整组删除 / 新增可重复字段组 |

形态：**官方仅向企业自建应用开放**（代开发与第三方均暂不支持）⇒ 零端点父接口 + 唯一自建子接口承载全部端点，**不设第三方 / 代开发子接口**（HR3，形态对齐 `IWechatWorkMsgAuditPermitUserService`、[Dial.md](../Dial/Dial.md)）。

E1 是 E2/E3 的前置：花名册字段全部以数字 `fieldid` 定位，**不存在**「按字段名读写」的形态，也不要把 `fieldid` 猜成证件号、手机号之类。

## 2. 前置条件

### 2.1 凭据与配置

| 配置项 | 值 | 校验时机 |
|---|---|---|
| 应用类型 | **仅** `WechatAppType.Internal`（第三方 / 代开发无子接口，官方暂不支持） | 注册期 `AddWechatApp` |
| 必填配置属性 | `CorpId` + `AgentId` + `AgentSecret` | `WechatAppConfig.Validate()` |
| 令牌类型 | `WechatTokenTypes.AccessToken` → 官方参数 `access_token`（HR4 断言两接口一致） | — |
| 令牌注入 | `TokenInjectionMode.Query` | 每次请求 |
| 令牌归属键 | 自建 `Wechat.AccessToken@Internal` | 运行期归属校验（错配抛 `WechatTokenOwnerMismatchException`） |

### 2.2 权限与业务前置

- **调用应用须配置到「人事助手 - 可调用接口的应用」中**（父/子两接口注释同文）。
- **目标员工须在调用应用的可见范围内**——E2/E3 均如此；E3 注释明确越界会返回错误码。
- E2 的 `userid` **官方必填**（DTO 侧类型是**非可空** `string`，是本仓少见的「请求体必填字段用非空类型」形态）。
- E3 的 `update_items` / `remove_items` / `insert_items` **三者不能全部为空**。
- **官方标注不支持更新的字段**：`11006`（年龄）、`11012`（社会工龄）、`12004`（员工状态）——这三项由其他字段派生，写它们不会生效。
- 花名册内容（手机号、住址、证件、家庭成员、合同）属**高敏个人数据**：示例一律假值；`value_string` / `value_mobile` / `value_file` 的取值**不得进日志、遥测或异常消息**。

### 2.3 官方开放的应用类型

| 端点 | 自建 | 第三方 | 代开发 |
|---|---|---|---|
| E1–E3 | ✅ | ❌（注释原文「暂不支持本接口」） | ❌（同） |

⇒ **不要**为第三方 / 代开发补子接口：HR3 锁「继承链上只有自建子接口」，这类「官方未开放 ⇒ 零端点」是本仓守卫的通用形态。

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkHrService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`，**零端点**） | 0 | 否 |
| `IWechatWorkInternalHrService` | `Mud.Wechat.Work` | 自建子接口（**承载全部 3 支**） | **3** | 是 |

注册组 `Hr`，装配入口 `AddHrApi()`（模块枚举 `WechatModule.Hr`）；子接口 `InheritedFrom = nameof(WechatWorkHrService)`。

## 4. 端点

### E1. `GetFieldsAsync` — 获取员工字段配置

```csharp
[Get("/cgi-bin/hr/get_fields")]
Task<GetHrFieldsResponse> GetFieldsAsync(CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 方法与路由 | `GET /cgi-bin/hr/get_fields` |
| 令牌 | Query `access_token`；**无业务参数** |

**响应参数**（`GetHrFieldsResponse`）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `group_list[]` | `GroupList` | `List<HrFieldGroup>?` | 字段组（如「个人信息」「在职信息」） |
| `group_list[].group_id` / `group_name` | `GroupId` / `GroupName` | `long?` / `string?` | 组 id / 组名 |
| `group_list[].field_list[]` | `FieldList` | `List<HrFieldConfig>?` | 组内字段 |
| `field_list[].fieldid` | `Fieldid` | `long?` | **字段 id**（官方示例拼写 `fieldid`，见 §5） |
| `field_list[].field_name` | `FieldName` | `string?` | 字段名称 |
| `field_list[].field_type` | `FieldType` | `long?` | **1 文本**（字符串或电话号码）/ **2 选项**（32 位非负整数）/ **3 时间**（64 位非负整数） |
| `field_list[].value_type` | `ValueType` | `long?` | **1 字符串 / 2 64 位非负整数 / 3 32 位非负整数 / 4 64 位整数 / 5 电话号码 / 6 文件** |
| `field_list[].is_must` | `IsMust` | `bool?` | 是否必填 |
| `field_list[].option_list[]` | `OptionList` | `List<HrFieldOption>?` | 选项枚举（**仅 `field_type = 2` 时返回**），元素 `id`（枚举值）+ `value`（文本） |

### E2. `GetStaffInfoAsync` — 获取员工花名册信息

```csharp
[Post("/cgi-bin/hr/get_staff_info")]
Task<GetHrStaffInfoResponse> GetStaffInfoAsync(
    [Body] GetHrStaffInfoRequest request, CancellationToken cancellationToken = default);
```

**请求参数**（`GetHrStaffInfoRequest`）

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `userid` | `Userid` | `string` | **是** | 目标员工（须在本应用可见范围内） |
| `get_all` | `GetAll` | `bool?` | 否 | `true` = 拉全部字段并**忽略 `fieldids`** |
| `fieldids` | `Fieldids` | `List<HrFieldQueryItem>?` | 否 | `get_all` 为 `false` 或不填时生效；每项 `fieldid` + `sub_idx` |

**响应参数**（`field_info[]` → `HrFieldInfo`）

| 字段 | 属性 | 类型 | 说明 |
|---|---|---|---|
| `fieldid` + `sub_idx` | `Fieldid` / `SubIdx` | `long?` | **两键定位一条字段值**；非可重复组 `sub_idx = 0` |
| `result` | `Result` | `long?` | **1 成功 / 2 失败 / 3 字段未找到 / 5 不支持获取的字段类型** |
| `value_type` | `ValueType` | `long?` | 决定下面哪个 `value_xxx` 有值 |
| `value_string` | `ValueString` | `string?` | `value_type = 1` |
| `value_uint64` | `ValueUint64` | `long?` | `value_type = 2`（**时间字段即此形态的时间戳**） |
| `value_uint32` | `ValueUint32` | `long?` | `value_type = 3`（**选项字段即此形态的枚举值**，对应 `option_list[].id`） |
| `value_int64` | `ValueInt64` | `long?` | `value_type = 4` |
| `value_mobile` | `ValueMobile` | `HrMobileValue?` | `value_type = 5`：`value_country_code` + `value_mobile`（均字符串） |
| `value_file` | `ValueFile` | `HrFileValue?` | `value_type = 6`：`media_id`（`List<string>?`，可经素材接口换取文件内容） |

### E3. `UpdateStaffInfoAsync` — 更新员工花名册信息

```csharp
[Post("/cgi-bin/hr/update_staff_info")]
Task<WechatWorkResponse> UpdateStaffInfoAsync(
    [Body] UpdateHrStaffInfoRequest request, CancellationToken cancellationToken = default);
```

| 字段 | 属性 | 类型 | 必填 | 说明 |
|---|---|---|---|---|
| `userid` | `Userid` | `string` | **是** | 目标员工（须在可见范围内） |
| `update_items[]` | `UpdateItems` | `List<HrUpdateFieldItem>?` | 条件 | 更新 / 增加 / **清空**单个字段（与下两项不可全空） |
| `remove_items[]` | `RemoveItems` | `List<HrRemoveFieldGroup>?` | 条件 | **整组删除**可重复字段组：`group_type` + `sub_idx` 均必填 |
| `insert_items[]` | `InsertItems` | `List<HrInsertFieldGroup>?` | 条件 | **新增一组**可重复字段组：`group_type` 必填 + `item[]` |

- `update_items[]`（`HrUpdateFieldItem`）＝ `fieldid`（必填）+ `sub_idx`（非可重复组填 `0`）+ 与字段类型匹配的那个 `value_xxx`（`value_string` / `value_uint64` / `value_uint32` / `value_int64` / `value_mobile`）。
- `group_type` 可重复字段组列表：**1 教育经历 / 2 工作经历 / 3 家庭成员 / 4 紧急联系人 / 5 合同信息**。
- `insert_items[].item` 的填写要求与 `update_items` 相同，但 **`sub_idx` 内容被忽略**；**找不到对应 `fieldid` 的字段内容被忽略**（不报错）。
- 电话号码在更新场景下**不填或空串 = 整个电话字段清空**。
- 响应无业务负载 ⇒ `WechatWorkResponse`（仅 `errcode` / `errmsg`）。

**调用示例**（E1 定位字段 → E2 按字段读 → E3 改一个字段并追加一组工作经历）

```csharp
using Mud.Wechat.Work;
using Mud.Wechat.Work.Abstractions;
using Mud.Wechat.Work.Abstractions.Enums;
using Mud.Wechat.Work.Abstractions.Exceptions;
using Mud.Wechat.Work.DataModels.Hr;

builder.Services.AddWechatApp(o =>
{
    o.AppKey = "hr-a";
    o.AppType = WechatAppType.Internal;
    o.CorpId = config["Wechat:CorpId"];
    o.AgentId = config["Wechat:AgentId"];
    o.AgentSecret = config["Wechat:AgentSecret"];
});
builder.Services.AddWechatWorkServices(b => b.AddHrApi());

var hr = provider.GetRequiredService<IWechatWorkInternalHrService>();
string userid = "zhangsan";   // 假值：须在应用可见范围内

// ① 取字段配置：花名册只认 fieldid，先建「字段名 → 配置」索引
var fields = await hr.GetFieldsAsync(cancellationToken);
WechatWorkException.ThrowIfFailed(fields, "/cgi-bin/hr/get_fields");

var fieldByName = new Dictionary<string, HrFieldConfig>(StringComparer.Ordinal);
foreach (HrFieldGroup group in fields.GroupList ?? new List<HrFieldGroup>())
{
    foreach (HrFieldConfig field in group.FieldList ?? new List<HrFieldConfig>())
    {
        if (field.FieldName is { Length: > 0 } name) fieldByName[name] = field;
    }
}
if (!fieldByName.TryGetValue("紧急联系人姓名", out HrFieldConfig? contactNameField))
{
    logger.LogWarning("花名册未配置「紧急联系人姓名」字段，跳过写入");
    return;
}

// ② 按字段读：get_all=false 时 fieldids 生效；逐条看 result（1 成功 / 2 失败 / 3 未找到 / 5 不支持的类型）
var read = await hr.GetStaffInfoAsync(
    new GetHrStaffInfoRequest
    {
        Userid = userid,
        GetAll = false,
        Fieldids = new List<HrFieldQueryItem>
        {
            new HrFieldQueryItem { Fieldid = contactNameField.Fieldid, SubIdx = 0 },
        },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(read, "/cgi-bin/hr/get_staff_info");

foreach (HrFieldInfo info in read.FieldInfo ?? new List<HrFieldInfo>())
{
    if (info.Result != 1)
    {
        logger.LogInformation("字段 {Fieldid} 组 {SubIdx} 读取未成功，result {Result}", info.Fieldid, info.SubIdx, info.Result);
        continue;
    }
    // value_type 决定取哪个 value_xxx：1 字符串 / 2、3、4 整数 / 5 电话 / 6 文件。
    // 值内容（姓名、手机号、附件）属个人数据，只记类型不记值。
    logger.LogInformation("字段 {Fieldid} 组 {SubIdx} 取回，值类型 {ValueType}", info.Fieldid, info.SubIdx, info.ValueType);
}

// ③ 写：更新一个普通字段 + 追加一组可重复字段组（group_type 4 = 紧急联系人）
var updated = await hr.UpdateStaffInfoAsync(
    new UpdateHrStaffInfoRequest
    {
        Userid = userid,
        UpdateItems = new List<HrUpdateFieldItem>
        {
            // 选项类型字段写 value_uint32（取 option_list[].id）；时间类型写 value_uint64 时间戳
            new HrUpdateFieldItem { Fieldid = contactNameField.Fieldid, SubIdx = 0, ValueString = "李四（假值）" },
        },
        InsertItems = new List<HrInsertFieldGroup>
        {
            new HrInsertFieldGroup
            {
                GroupType = 4,   // 1 教育 / 2 工作 / 3 家庭成员 / 4 紧急联系人 / 5 合同信息
                Item = new List<HrUpdateFieldItem>
                {
                    // insert 场景 sub_idx 会被官方忽略；不存在的 fieldid 对应内容被静默丢弃
                    new HrUpdateFieldItem { Fieldid = contactNameField.Fieldid, ValueString = "王五（假值）" },
                },
            },
        },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(updated, "/cgi-bin/hr/update_staff_info");
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `fieldid` vs `field_id` | 官方**字段说明表**把字段 id 记作 `field_id`，但**请求/响应示例**与另两支端点请求侧均为 `fieldid`；本仓**以示例为准**（HR2 逐字锁定） | 照参数表写 `field_id` → 反序列化丢字段 |
| `field_type` vs `value_type` | 前者是字段的**语义类型**（1 文本 / 2 选项 / 3 时间），后者是**值的承载类型**（1~6）；两者不同维度 | 拿 `field_type` 去 switch `value_xxx` → 电话/文件分支永远取不到 |
| 六个 `value_xxx` | **只有一个**按 `value_type` 有值，其余为 `null` | 只读 `ValueString` → 时间/选项/电话字段恒为空 |
| 时间字段的值 | 落在 `value_uint64`（时间戳），**不是** `value_int64` | 读到 null 以为官方没返回 |
| 选项字段的值 | 落在 `value_uint32`，须回 E1 的 `option_list[].id` 换文本 | 直接把数字当展示值写回 → 越界或错选项 |
| `sub_idx` | **两键定位**的一部分：非可重复组固定 `0`；`insert_items[].item` 里的 `sub_idx` **被忽略** | 读可重复组时不带 `sub_idx` → 同字段多条记录被当成一条 |
| 「清空」的三种写法 | `update_items` 里给字段置空值可清空；电话字段**不填/空串 = 整个电话清空**；`remove_items` 是**整组删除** | 想清空单字段却用了 `remove_items` → 整组没了 |
| 不支持更新的字段 | `11006` 年龄、`11012` 社会工龄、`12004` 员工状态（注释原文） | 反复重试写入却无变化 |
| 文件字段只读 | `HrFileValue`（`value_file.media_id`）只出现在**响应侧**；`HrUpdateFieldItem` **没有** `value_file` ⇒ 本端点不写文件字段 | 按「读写对称」假设去写文件 → 类型上就没有该属性 |
| `get_all` | `true` 时**忽略 `fieldids`** | 两个都给却以为只拉指定字段 |
| E1 的动词 | 官方即 **GET 且无业务参数**（本域另两支是 POST） | 给 E1 造请求体 → 打坏 HR1 与 G5 |
| 可见范围 | 目标员工不在本应用可见范围 ⇒ E2/E3 报错 | 用全企业 userid 直接调 |
| 应用类型 | **仅自建**：第三方 / 代开发子接口不存在 | 按三类形态注入 → `InvalidOperationException` |

SDK 侧通用约束：**不自动判错**（`errcode != 0` 时 HTTP 仍 200）、**不做本地必填校验**、**不校验 `fieldid` 是否存在**（不存在的 `fieldid` 在 insert 场景被静默忽略）、**不做逐字段成功判定**（须看 `field_info[].result`）；令牌走 Query 是官方契约（`MUD005`）。

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 成功 | E2 成功**不等于每个字段都取到**：逐条看 `field_info[].result` |
| 40014 / 42001 / 42007 | `InvalidAccessToken` / `ExpiredAccessToken` / `RelatedAccessTokenInvalid` | 令牌不合法 / 过期 / 关联失效 | 自愈集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | **不自愈**，属装配缺陷 |

`result` 的四个取值（1/2/3/5）是**字段级**结果码，与信封 `errcode` 不是一套。E3 注释说「员工不在可见范围会返回错误码」但**未给具体码** ⇒ 见 §7。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 业务错误码全集 | 注释未逐条摘录 ⇒ **未逐项核验**（含「员工不可见」「字段不可写」的码） | 官方 99131 / 99132 / 99133 + 全局错误码页 |
| `fieldid` 的官方枚举表 | 注释只给三个「不支持更新」的 id，**未给字段 id 全集** ⇒ 未核验；任何按 id 硬编码的业务分支都不可移植 | 官方 99131 字段说明表 |
| 端点频率限制 | 注释未给 QPS ⇒ **未逐项核验**，勿从审批域「600 次/分」借数值 | 官方全局频率附录 |
| `value_file` 的写入路径 | 更新 DTO 无该属性 ⇒ **本域不建模文件写入**；官方是否支持未核验 | 官方 99133 参数表 |
| `option_list` 在更新时的校验强度 | 注释只说选项类型取值是 32 位非负整数，未说明填了不存在的枚举值会报错还是被忽略 ⇒ 未核验 | 官方 99131 / 99133 |
| 可重复字段组的数量上限 | `insert_items` / `remove_items` 的组数与每组字段数上限注释未给 ⇒ 未逐项核验 | 官方 99133 |
| 与「通讯录」字段的关系 | 花名册字段与通讯录 `extattr` / 自定义字段是否同源，本域注释未说明 ⇒ 未核验 | 官方 99131 + [Contacts/Users.md](../Contacts/Users.md) |

## 8. 延伸阅读

- 域总览：[Src/Work/Mud.Wechat.Work/Interfaces/Hr/README.md](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Hr/README.md)（端点速览、令牌表、逐端点契约明细）
- 同域邻面：花名册字段的**人员主数据**（userid / 部门 / 状态）在 [Contacts/Users.md](../Contacts/Users.md)；`value_file.media_id` 的下载属素材接口（本域不承载）
- 同形态参照（零端点父接口 + 仅自建子接口承载）：[Dial.md](../Dial/Dial.md)、[ServedContact.md](../ExternalContact/ServedContact.md)、通讯录规则 `IWechatWorkContactRulesService`
- 官方页：[获取员工字段配置（99131）](https://developer.work.weixin.qq.com/document/path/99131) · [获取员工花名册信息（99132）](https://developer.work.weixin.qq.com/document/path/99132) · [更新员工花名册信息（99133）](https://developer.work.weixin.qq.com/document/path/99133)
- 文档规范：[Documents/_spec/文档编写规范.md](../../../../_spec/文档编写规范.md)
