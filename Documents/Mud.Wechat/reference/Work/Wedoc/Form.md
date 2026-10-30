# 收集表 · 创建 / 编辑 / 信息 / 统计 / 答案

| 项 | 值 |
|---|---|
| 所属产品线 | 企业微信（WeCom）· `Work` 线 · `Wedoc` 域 · 管理收集表族 |
| NuGet 包 / 版本 | `Mud.Wechat.Work` / `1.0.0` |
| 接口源文件 | [IWechatWorkWedocFormService.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/Form/IWechatWorkWedocFormService.cs)（父，承载全部 5 支）＋ 自建 [IWechatWorkWedocFormService_Internal.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/Form/IWechatWorkWedocFormService_Internal.cs) / 第三方 [_ThirdParty.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/Form/IWechatWorkWedocFormService_ThirdParty.cs) / 代开发 [_Provider.cs](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/Form/IWechatWorkWedocFormService_Provider.cs)（三个零端点子接口） |
| DTO 源文件 | [DataModels/Wedoc/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Wedoc/) 与 [RequestModel/](../../../../../Src/Work/Mud.Wechat.Work/DataModels/Wedoc/RequestModel/)（本族 5 个请求 + 4 个专用响应 + 收集表 30 余型，`SerializerClassName = Wedoc` → `WedocJsonContext`） |
| 契约守卫 | [WechatWedocContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatWedocContractGuards.cs)（WD13 本族 5 路由 / 九族 69 端点总表 / WD4 层级 / WD5 令牌 / WD7 DTO 292 型登记）＋ G5 ＋ TO1~TO3 ＋ N1~N3 |
| 官方文档 | 自建＋第三方：[97462](https://developer.work.weixin.qq.com/document/path/97462) / [97816](https://developer.work.weixin.qq.com/document/path/97816) / [97817](https://developer.work.weixin.qq.com/document/path/97817) / [97818](https://developer.work.weixin.qq.com/document/path/97818) / [97819](https://developer.work.weixin.qq.com/document/path/97819)；代开发：[97472](https://developer.work.weixin.qq.com/document/path/97472) / [97820](https://developer.work.weixin.qq.com/document/path/97820) / [97821](https://developer.work.weixin.qq.com/document/path/97821) / [97822](https://developer.work.weixin.qq.com/document/path/97822) / [97823](https://developer.work.weixin.qq.com/document/path/97823) |
| 状态 | `已核验` |
| 本文核验日期 | 2026-10-30（内部证据：守卫 + 接口/DTO XML 注释；官方页本批未重新渲染取文） |
| 示例代码验证 | 2026-10-30 在本仓临时 Web SDK 工程（`net8.0`）与本批 32 篇的可运行示例合并逐字编译，0 错误（本篇 1 段；本批 32 段、全仓累计 203 段）；仅编译级验证，未做真账号运行验证 |
| 证据基础 | 内部：WD13 路由表 + 5 支端点 XML 注释 + 题型/设置 DTO 注释 + [域 README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/README.md) 第 11 节（2026-10-18）· 官方：本批未重新取文 |

## 1. 这个接口族做什么

收集表（`create_form` 建出来的对象）是一条**四步生命周期**：

1. **建**（E1）→ 拿到 `formid`。
2. **改**（E2）：`oper = 1` 全量替换问题、`oper = 2` 全量替换设置——都是**整组替换**，不是增量。
3. **读结构**（E3）→ 拿 `form_info` 与 **`repeated_id`**。
4. **读数据**（E4 统计 / E5 答案）：都用 `repeated_id` 定位，**不是 `formid`**。

| 端点 | 定位键 | 作用 |
|---|---|---|
| E1 `create_form` | 无 | 建表，返回 `formid` |
| E2 `modify_form` | `formid` | 全量替换问题（`oper=1`）或设置（`oper=2`） |
| E3 `get_form_info` | `formid` | 读结构，返回 `form_info` + `repeated_id[]` |
| E4 `get_form_statistic` | **`repeated_id`** | 统计结果 / 已提交列表 / 未提交列表 |
| E5 `get_form_answer` | **`repeated_id`** + `answer_ids[]` | 读答案明细 |

**`formid` 与 `repeated_id` 分治是本族最容易踩的一脚**：`formid` 标识收集表本身，`repeated_id` 标识**一个收集周期**——开了定时重复的收集表会有多个 `repeated_id`，每期的统计与答案互相隔离。

与其它族的边界：本族只服务收集表；`doc_type` 为 3 / 4 / 10 / 11 的文档、表格、智能表格、智能文档分别走 [Wedoc.md](./Wedoc.md) 与各内容族。收集表**不能**用 `create_doc` 创建（`doc_type` 无收集表取值），也不能用文档权限族管理。

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

- 自建应用须用「可调用应用」列表内 secret 换取的 `access_token`；第三方 / 代开发须具有「**文档**」权限；**仅可操作该应用自己创建的收集表**。
- `spaceid` / `fatherid` 决定落位：`fatherid` 为空时建在**根目录**（此时 `spaceid` 即空间 id）；给了 `fatherid` 则建在该父目录下。
- **问题规模闸**：问题数组 ≤**200**；`question_id` **从 1 开始**（家校范围收集表**从 2 开始**）；`pos` 也从 1 开始。
- **题型参数闸**：文本题 `char_len` 必须 >0 且 ≤**4000**；图片 / 文件题数量 [1, 9]（默认 9）、单文件 ≤**3000MB**；多选题 `number` 不得超过选项数，`type` 为 1/2/3 时必须给且 >0；时长题 `day_range` [1, 24]（默认 24）。
- **定时闸**：定时重复与定时结束**互斥**，两者都填时**以重复为准**；`week_flag` 只在 `repeat_type = 0`（每周）可填、`skip_holiday` 只在 `= 1`（每天）可填、`day_of_month` 只在 `= 2`（每月）可填；开启定时重复后 `fill_in_range` **必填**。
- **统计闸**：`limit` ≤**10000**；`start_time` / `end_time` **只在 `req_type = 2`（已提交列表）时必填**，且以整天为界（00:00:00 与 23:59:59）；`cursor` 首次不传。
- **答案闸**：`answer_ids` 单批 ≤**100**；`answer_status = 3` 表示答案已被统计者移除或删除。
- **匿名口径**：匿名填写时 `userid` / `user_name` / `tmp_external_userid` **都不返回**，代码不能假设三者非空。
- **数据安全**：`user_name`、`userid`、`tmp_external_userid`、答案正文（`text_reply` 等）全是个人数据，**日志只记 `answer_id`、条数与错误码**，不得打印姓名与作答内容。`tmp_external_userid` 同一外部用户在不同收集表中不一致，须经身份转换接口换成 `external_userid` 才能识别。

### 2.3 官方开放的应用类型

三类应用端点完全一致，官方只给两页（自建与第三方共用）：

| 端点 | 自建 | 第三方 | 代开发 | 文档号（自建＋第三方 / 代开发） |
|---|---|---|---|---|
| E1 创建收集表 | ✅ | ✅ | ✅ | 97462 / 97472 |
| E2 编辑收集表 | ✅ | ✅ | ✅ | 97816 / 97820 |
| E3 获取收集表信息 | ✅ | ✅ | ✅ | 97817 / 97821 |
| E4 收集表的统计信息查询 | ✅ | ✅ | ✅ | 97818 / 97822 |
| E5 读取收集表答案 | ✅ | ✅ | ✅ | 97819 / 97823 |

## 3. 接口族与注册

| 接口 | 命名空间 | 角色 | 声明端点数 | 可注入 DI |
|---|---|---|---|---|
| `IWechatWorkWedocFormService` | `Mud.Wechat.Work.Interfaces` | 公共父接口（`IsAbstract = true`） | **5** | 否 |
| `IWechatWorkInternalWedocFormService` | `Mud.Wechat.Work` | 自建子接口（空标记） | 0 | 是 |
| `IWechatWorkThirdPartyWedocFormService` | `Mud.Wechat.Work` | 第三方子接口（空标记） | 0 | 是 |
| `IWechatWorkProviderWedocFormService` | `Mud.Wechat.Work` | 代开发子接口（空标记） | 0 | 是 |

注册组 `Wedoc`、装配入口 `AddWedocApi()`；子接口 `InheritedFrom = nameof(WechatWorkWedocFormService)`。

## 4. 端点

> 以下 5 支的凭据面完全一致（`Wechat.AccessToken` + Query `access_token` + `application/json` 源生成序列化），表中不再重复；每支均只能作用于**应用自建**的收集表。

### E1. `CreateFormAsync` — 创建收集表

```csharp
[Post("/cgi-bin/wedoc/create_form")]
Task<CreateWedocFormResponse> CreateFormAsync(
    [Body] CreateWedocFormRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/create_form`（**不是** `create_doc`） |
| 官方文档 | 97462 / 97472 |

**请求**（`CreateWedocFormRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `spaceid` | `Spaceid` | `string?` | 空间 id |
| `fatherid` | `Fatherid` | `string?` | 父目录 fileid；根目录时不传，此时空间即 `spaceid` |
| `form_info` | `FormInfo` | `WedocFormInfo?` | 必填 |
| `form_info.form_title` | — | `string?` | 标题，创建时必填 |
| `form_info.form_desc` | — | `string?` | 描述 |
| `form_info.form_header` | — | `string?` | 表头背景图链接 |
| `form_info.form_question.items[]` | — | `List<WedocFormItem>?` | 问题列表，创建时必填 |
| `form_info.form_setting` | — | `WedocFormSetting?` | 填写权限 / 定时 / 匿名等 |

**问题项**（`WedocFormItem`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `question_id` | `QuestionId` | `uint?` | 从 1 开始（家校从 2） |
| `title` | `Title` | `string?` | 问题描述 |
| `pos` | `Pos` | `uint?` | 序号，从 1 开始 |
| `status` | `Status` | `uint?` | 1 正常 / 2 被删除 |
| `reply_type` | `ReplyType` | `uint?` | 1 文本 / 2 单选 / 3 多选 / 5 位置 / 9 图片 / 10 文件 / 11 日期 / 14 时间 / 15 下拉 / 16 体温 / 17 签名 / 18 部门 / 19 成员 / 22 时长 |
| `must_reply` | `MustReply` | `bool?` | 是否必答 |
| `note` | `Note` | `string?` | 备注 |
| `placeholder` | `Placeholder` | `string?` | 填写提示（响应返回） |
| `option_item[]` | `OptionItem` | `List<WedocFormOptionItem>?` | 选项（`key` / `value` / `status`） |
| `question_extend_setting` | `QuestionExtendSetting` | `WedocFormExtendSetting?` | 按 `reply_type` 取对应 `*_setting` |

`question_extend_setting` 的成员与题型对应：`text_setting`(1) / `radio_setting`(2) / `checkbox_setting`(3) / `location_setting`(5) / `image_setting`(9) / `file_setting`(10) / `date_setting`(11) / `time_setting`(14) / `duration_setting`(22) / `temperature_setting`(16) / `department_setting`(18) / `member_setting`(19)。**下拉列表（15）与签名（17）无额外设置**（官方未给）。

**响应**：`formid`（`string?`）。

**注意**：题型编号**不连续**（缺 4 / 6~8 / 12~13 / 20~21），这是官方枚举原文，不要自行补齐。创建后要拿 `repeated_id` 必须再调 E3。

### E2. `ModifyFormAsync` — 编辑收集表

```csharp
[Post("/cgi-bin/wedoc/modify_form")]
Task<WechatWorkResponse> ModifyFormAsync(
    [Body] ModifyWedocFormRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/modify_form`（本族用 `modify_`，与文档权限族的 `mod_` 不同） |
| 官方文档 | 97816 / 97820 |

**请求**（`ModifyWedocFormRequest`）：`oper`（必填）、`formid`（必填）、`form_info`（按 `oper` 选传对应字段组）。

**响应**：仅 `errcode` / `errmsg`。

**注意**：**两种 `oper` 都是全量替换**——`oper = 1` 替换问题（标题 / 描述 / 表头 / 问题列表），`oper = 2` 替换设置。**没提到的字段会被清掉或回到默认**，因此改单个问题也要把整组问题带上（先 E3 读、改完再整体提交）。家校范围收集表的 `fill_out_auth` **无法修改**。

### E3. `GetFormInfoAsync` — 获取收集表信息

```csharp
[Post("/cgi-bin/wedoc/get_form_info")]
Task<GetWedocFormInfoResponse> GetFormInfoAsync(
    [Body] GetWedocFormInfoRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/get_form_info` |
| 官方文档 | 97817 / 97821 |

**请求**：`formid`（必填）。**响应**（`GetWedocFormInfoResponse`）：`form_info`（`WedocFormInfo`）/ `repeated_id`（**`List<string>?`**，收集周期 id 列表）。

**注意**：这是取 `repeated_id` 的**唯一**入口，E4 / E5 必须先经过它。定时重复的收集表 `repeated_id` 有多个，取哪个周期要由业务决定。

### E4. `GetFormStatisticAsync` — 收集表的统计信息查询

```csharp
[Post("/cgi-bin/wedoc/get_form_statistic")]
Task<GetWedocFormStatisticResponse> GetFormStatisticAsync(
    [Body] GetWedocFormStatisticRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/get_form_statistic` |
| 官方文档 | 97818 / 97822 |

**请求**（`GetWedocFormStatisticRequest`）

| 字段（JSON） | 属性 | 类型 | 说明 |
|---|---|---|---|
| `repeated_id` | `RepeatedId` | `string?` | **周期 id**，来自 E3 |
| `req_type` | `ReqType` | `uint?` | 必填：1 仅统计结果 / 2 已提交列表 / 3 未提交列表 |
| `start_time` | `StartTime` | `ulong?` | 仅 `req_type = 2` 必填，当天 00:00:00 起 |
| `end_time` | `EndTime` | `ulong?` | 仅 `req_type = 2` 必填，当天 23:59:59 止 |
| `limit` | `Limit` | `ulong?` | 批次 ≤10000 |
| `cursor` | `Cursor` | `ulong?` | 首次不传 |

**响应**（`GetWedocFormStatisticResponse`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `fill_cnt` | `FillCnt` | `ulong?` | 已填写次数 |
| `fill_user_cnt` | `FillUserCnt` | `ulong?` | 已填写人数 |
| `unfill_user_cnt` | `UnfillUserCnt` | `ulong?` | 未填写人数 |
| `submit_users[]` | `SubmitUsers` | `List<WedocFormSubmitUser>?` | `req_type = 2` 时返回（`userid` / `tmp_external_userid` / `submit_time` / `answer_id` / `user_name`） |
| `unfill_users[]` | `UnfillUsers` | `List<WedocFormUnfillUser>?` | `req_type = 3` 时返回（`userid` / `user_name`） |
| `has_more` | `HasMore` | `bool?` | 是否还有更多 |
| `cursor` | `Cursor` | `ulong?` | 下轮的 `cursor` |

**注意**：`has_more` 在本族是**布尔**（与智能文档族的字符串 `has_more` 不同）。**未提交列表只在收集表限制了提交范围时才有结果**，全员可填时 `unfill_users` 恒空。`submit_users[].answer_id` 可直接喂给 E5。

### E5. `GetFormAnswerAsync` — 读取收集表答案

```csharp
[Post("/cgi-bin/wedoc/get_form_answer")]
Task<GetWedocFormAnswerResponse> GetFormAnswerAsync(
    [Body] GetWedocFormAnswerRequest request,
    CancellationToken cancellationToken = default);
```

| 项 | 值 |
|---|---|
| 路由 | `POST /cgi-bin/wedoc/get_form_answer` |
| 官方文档 | 97819 / 97823 |

**请求**：`repeated_id`（**周期 id**）、`answer_ids`（`List<ulong>`，必填，单批 ≤100）。

**响应**（`GetWedocFormAnswerResponse`）：`answer.answer_list[]`（`WedocFormAnswerItem`）

| 字段（JSON） | 属性（C#） | 类型 | 说明 |
|---|---|---|---|
| `answer_id` | `AnswerId` | `ulong?` | 答案 id |
| `user_name` | `UserName` | `string?` | 填写人名字，**匿名不返回** |
| `userid` | `Userid` | `string?` | 企业成员 id，**匿名不返回** |
| `tmp_external_userid` | `TmpExternalUserid` | `string?` | 外部用户临时 id，**匿名不返回** |
| `ctime` / `mtime` | `Ctime` / `Mtime` | `ulong?` | 创建 / 修改时间戳 |
| `answer_status` | `AnswerStatus` | `uint?` | 1 正常 / 3 已被移除或删除 |
| `reply.items[]` | `Reply` | `WedocFormReply?` | `question_id` / `text_reply` / `option_reply` / `option_extend_reply` / `file_extend_reply` / `department_reply` / `member_reply` / `duration_reply` |

**注意**：`answer_status = 3` 的答案**仍会返回**（不报错），但内容已作废，业务侧必须过滤。题型 ↔ 作答字段：文本类（文本 / 位置 / 日期 / 时间 / 体温 / 签名）走 `text_reply`；单选 / 多选 / 下拉走 `option_reply`（选项 `key` 列表，多选多个）+ `option_extend_reply`（「其他」）；文件 / 图片走 `file_extend_reply`；部门 / 成员 / 时长各有专属对象。

**调用示例**（建表 → 读结构取 `repeated_id` → 统计 → 取答案）

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

// 第三方 / 代开发改用 IWechatWorkThirdPartyWedocFormService / IWechatWorkProviderWedocFormService
// 并在调用前 switcher.UseCorpScope(appKey, authCorpId, permanentCode)。
var form = provider.GetRequiredService<IWechatWorkInternalWedocFormService>();
string spaceId = "SPACEID-FAKE-PLACEHOLDER";

// ① 建表：question_id 与 pos 都从 1 开始；单选 option_item 的 key 自 1 起
var created = await form.CreateFormAsync(
    new CreateWedocFormRequest
    {
        Spaceid = spaceId,
        FormInfo = new WedocFormInfo
        {
            FormTitle = "示例收集表",
            FormDesc = "仅用于文档示例（假数据）。",
            FormQuestion = new WedocFormQuestion
            {
                Items =
                [
                    new WedocFormItem
                    {
                        QuestionId = 1,
                        Pos = 1,
                        Status = 1,
                        Title = "请填写说明",
                        ReplyType = 1,
                        MustReply = true,
                        QuestionExtendSetting = new WedocFormExtendSetting
                        {
                            TextSetting = new WedocFormTextSetting { ValidationType = 0, CharLen = 200 },
                        },
                    },
                    new WedocFormItem
                    {
                        QuestionId = 2,
                        Pos = 2,
                        Status = 1,
                        Title = "请选择一项",
                        ReplyType = 2,
                        MustReply = false,
                        OptionItem =
                        [
                            new WedocFormOptionItem { Key = 1, Value = "选项甲", Status = 1 },
                            new WedocFormOptionItem { Key = 2, Value = "选项乙", Status = 1 },
                        ],
                    },
                ],
            },
            FormSetting = new WedocFormSetting
            {
                AllowMultiFill = false,
                CanAnonymous = false,
            },
        },
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(created, "/cgi-bin/wedoc/create_form");
string formId = created.Formid ?? throw new InvalidOperationException("响应未返回 formid");

// ② 读结构：repeated_id 是统计与答案的定位键，不是 formid
var info = await form.GetFormInfoAsync(
    new GetWedocFormInfoRequest { Formid = formId },
    cancellationToken);
WechatWorkException.ThrowIfFailed(info, "/cgi-bin/wedoc/get_form_info");
string repeatedId = info.RepeatedId?.FirstOrDefault()
    ?? throw new InvalidOperationException("响应未返回 repeated_id");

// ③ 只取统计结果（req_type = 1）：不传 start_time / end_time
var statistic = await form.GetFormStatisticAsync(
    new GetWedocFormStatisticRequest { RepeatedId = repeatedId, ReqType = 1 },
    cancellationToken);
WechatWorkException.ThrowIfFailed(statistic, "/cgi-bin/wedoc/get_form_statistic");
logger.LogInformation("已填 {Fill} 次 / {Users} 人", statistic.FillCnt, statistic.FillUserCnt);

// ④ 取已提交列表（req_type = 2）：时间范围必填，以整天为界
var submitted = await form.GetFormStatisticAsync(
    new GetWedocFormStatisticRequest
    {
        RepeatedId = repeatedId,
        ReqType = 2,
        StartTime = 1761696000,   // 假数据：某日 00:00:00
        EndTime = 1761782399,     // 假数据：同日 23:59:59
        Limit = 100,
    },
    cancellationToken);
WechatWorkException.ThrowIfFailed(submitted, "/cgi-bin/wedoc/get_form_statistic");

// ⑤ 按 answer_id 读答案（单批 ≤100）：日志只记条数，不打印姓名与作答内容
var answerIds = (submitted.SubmitUsers ?? [])
    .Select(u => u.AnswerId)
    .Where(id => id.HasValue)
    .Select(id => id!.Value)
    .Take(100)
    .ToList();
if (answerIds.Count > 0)
{
    var answers = await form.GetFormAnswerAsync(
        new GetWedocFormAnswerRequest { RepeatedId = repeatedId, AnswerIds = answerIds },
        cancellationToken);
    WechatWorkException.ThrowIfFailed(answers, "/cgi-bin/wedoc/get_form_answer");
    int valid = (answers.Answer?.AnswerList ?? []).Count(a => a.AnswerStatus != 3);
    logger.LogInformation("读取 {Count} 份答案，其中有效 {Valid} 份", answerIds.Count, valid);
}
```

## 5. 陷阱与易混对照

| 易混对象 | 区别点 | 用错的后果 |
|---|---|---|
| `formid` vs `repeated_id` | E2 / E3 用 `formid`；E4 / E5 用 `repeated_id` | 拿 `formid` 查统计 ⇒ 失败或空 |
| `repeated_id` 是列表 | E3 返回 `List<string>`；定时重复的收集表有多个周期 | 只取第一个 ⇒ 统计漏期 |
| `create_form` ≠ `create_doc` | 收集表不走 `doc_type` | 用 `create_doc` 建不出收集表 |
| `modify_` vs `mod_` | 本族 `modify_form`；文档权限族 `mod_*` | 拼错 ⇒ 404 |
| 全量替换语义 | E2 的 `oper = 1` / `2` 都是整组替换 | 只传一个字段 ⇒ 其余被清 |
| 家校范围 | `fill_out_auth` 不可改、`question_id` 从 2 开始 | 按 1 起写 ⇒ 冲突 |
| 题型编号不连续 | 1/2/3/5/9/10/11/14/15/16/17/18/19/22 | 自行补编号 ⇒ 题型错乱 |
| 定时重复 vs 定时结束 | 互斥；都填以重复为准 | 以为两者可叠加 |
| 重复子字段 | `week_flag` 仅每周、`skip_holiday` 仅每天、`day_of_month` 仅每月 | 错配 ⇒ 不生效 |
| 开启重复 | `fill_in_range` 必填 | 漏填 ⇒ 创建失败 |
| `start_time` / `end_time` | 只在 `req_type = 2` 必填，其它类型**不要传** | `req_type = 1` 传时间 ⇒ 参数错误 |
| `has_more` 形状 | 本族是 `bool`；智能文档族是 `string` | 当字符串比较 ⇒ 编译错 |
| 未提交名单 | 仅当限制了提交范围才有结果 | 全员可填时误判「全员已交」 |
| `answer_status = 3` | 答案已被移除 / 删除，但仍返回 | 不过滤 ⇒ 统计虚高 |
| 匿名作答 | `userid` / `user_name` / `tmp_external_userid` 全为空 | 用 `userid!` 解引用 ⇒ NRE |
| `tmp_external_userid` | 同一外部用户在不同收集表中不一致 | 跨表当主键 ⇒ 认错人 |
| 个人信息 | 姓名、userid、作答正文 | 落日志 ⇒ 泄露 |

## 6. 失败处理与错误码

| `errcode` | 常量 / 来源 | 含义 | 与本域的关系 |
|---|---|---|---|
| 0 | `WechatErrorCodes.Success` | 请求成功 | 判 `IsSuccess` |
| 640017 / 640018 / 640008 | 域 README 留档 | 未开启 API / 未授编辑权限 / 无管理权限 | 权限前置 |
| 40014 / 42001 / 42007 | 令牌失效码 | 应用令牌类错误 | 统一失效码处理 |
| 42011 | `InvalidProviderAccessToken` | 服务商令牌不合法 / 过期 | 失效码集合 |
| 41001 | `MissingAccessToken` | 缺少凭据参数 | 不自愈，装配缺陷 |

题型参数越界、`repeated_id` 无效、`answer_ids` 超批的具体业务码官方页未逐项留档（§7）。其余见[全局错误码](https://developer.work.weixin.qq.com/document/path/90455)（90455）与[访问频率限制](https://developer.work.weixin.qq.com/document/path/90454）（90454）。

## 7. 已知留档缺口

| 缺口 | 说明 | 需要时去哪查 |
|---|---|---|
| 题型 `reply_type` 完整语义 | 注释列了 14 个取值，`18` 部门 / `19` 成员 / `22` 时长 的应答形态未逐项成文 | 官方 97462 |
| 各题型 `*_setting` 字段全集 | 文本题注释最全，位置 / 图片 / 文件 / 日期 / 时间 / 体温 / 部门 / 成员仅列为成员 | 官方 97462 |
| `fill_out_auth` 取值语义 | 注释只有字段名，没有取值表 | 官方 97816 |
| `setting_manager_range` 与 `fill_in_range` 差异 | 前者不使用 `departmentids`（注释明示），其余差异未成文 | 官方 97462 |
| 编辑后的 `repeated_id` 变化 | 改设置（如开定时重复）后周期 id 是否重生 ⇒ 未核验 | 官方 97816 |
| 本族频率限制 | 官方未给独立数值频率 ⇒ 走全局 | 90454 |
| 真账号验证 | 本批仅内部证据核验 | — |

## 8. 延伸阅读

- 同域兄弟：[Wedoc.md](./Wedoc.md)（`create_doc` 建的四类文档对象，**不含收集表**）· [DocPermission.md](./DocPermission.md)（四类文档的权限，**不覆盖收集表**）
- 内容族（均与收集表无关）：[Document.md](./Document.md) · [Spreadsheet.md](./Spreadsheet.md) · [SmartSheet.md](./SmartSheet.md) · [SmartDoc.md](./SmartDoc.md)
- 域总览：[Wedoc README](../../../../../Src/Work/Mud.Wechat.Work/Interfaces/Wedoc/README.md)（第 11 节「收集表」）
- 官方页：[创建收集表 97462](https://developer.work.weixin.qq.com/document/path/97462) · [编辑收集表 97816](https://developer.work.weixin.qq.com/document/path/97816) · [获取收集表信息 97817](https://developer.work.weixin.qq.com/document/path/97817) · [统计信息查询 97818](https://developer.work.weixin.qq.com/document/path/97818) · [读取收集表答案 97819](https://developer.work.weixin.qq.com/document/path/97819)
