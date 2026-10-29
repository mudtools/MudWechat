# School 域 · 家校（校园）

企业微信「家校沟通」模块：本仓共落地 **45 支端点**，分属八个接口族——家校沟通基础族 **7 支**（守卫 SCH1a：[Basic/IWechatWorkSchoolService.cs](./Basic/IWechatWorkSchoolService.cs)）、家校管理配置族 **3 支**（SCH1b：[Setting/IWechatWorkSchoolSettingService.cs](./Setting/IWechatWorkSchoolSettingService.cs)）、学生与家长管理族 **16 支**（SCH1c：[User/IWechatWorkSchoolUserService.cs](./User/IWechatWorkSchoolUserService.cs)）、部门管理族 **5 支**（SCH1e：[Department/IWechatWorkSchoolDepartmentService.cs](./Department/IWechatWorkSchoolDepartmentService.cs)）、网页授权登录族 **3 支**（SCH1d：[Auth/IWechatWorkSchoolAuthService.cs](./Auth/IWechatWorkSchoolAuthService.cs) 1 支 + [Auth/IWechatWorkSchoolAuthService_ThirdParty.cs](./Auth/IWechatWorkSchoolAuthService_ThirdParty.cs) 2 支）、健康上报族 **4 支**（SCH1f：[HealthReport/IWechatWorkSchoolHealthReportService.cs](./HealthReport/IWechatWorkSchoolHealthReportService.cs)）、上课直播族 **5 支**（SCH1g：[Living/IWechatWorkSchoolLivingService.cs](./Living/IWechatWorkSchoolLivingService.cs)）、班级收款族 **2 支**（SCH1h：[ClassPay/IWechatWorkSchoolClassPayService.cs](./ClassPay/IWechatWorkSchoolClassPayService.cs)）。
注册组 `School`，模块枚举 `WechatModule.School`（[`AddSchoolApi()`](../../Extensions/WechatWorkServiceBuilder.cs)）。
逐端点核验 2026-10-18。

> **事实来源限定**：本文全部断言只来自本仓三类证据——本目录（含子目录）28 份接口文件的 XML 注释、
> [WechatSchoolContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatSchoolContractGuards.cs)（SCH1a~SCH1h / SCH2 / SCH3 / SCH4）、
> [Src/Work/Mud.Wechat.Work/DataModels/School/](../../../Mud.Wechat.Work/DataModels/School/) 下的 DTO 声明；
> 端点数与路由以守卫断言为准，官方文档号仅作定位标识、不作为本文事实来源。
> 未能在本仓三类证据内核验的项目一律集中在文末「已知留档缺口」，标为**未逐项核验**，正文不作推断。

## 端点速览

令牌/开放面列写本仓真实契约：`access_token` 恒为 Query 注入（SCH3，MUD005 已知接受风险，无法改 Header）；
除网页授权登录域第三方走 `suite_access_token`，其余全族令牌路由键恒为 `Wechat.AccessToken`。
「开放面」指官方对该端点开放的应用类型，由 SCH2 的层级断言锁死。计数以守卫为准。

### 家校沟通基础族（SCH1a，7 支，父接口收敛 + 三类空标记子接口）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `GetSubscribeQrCodeAsync` | GET | `/cgi-bin/externalcontact/get_subscribe_qr_code` | `access_token`(Query)；自建+第三方+代开发 | 92320/92197/96719；返回大中小三尺寸二维码 |
| 2 | `SetSubscribeModeAsync` | POST | `/cgi-bin/externalcontact/set_subscribe_mode` | 同上；自建+第三方 | 92318/92290；`subscribe_mode` 1 可扫码加入 / 2 禁止 |
| 3 | `GetSubscribeModeAsync` | GET | `/cgi-bin/externalcontact/get_subscribe_mode` | 同上；自建+第三方 | 92318/92290；官方即 GET |
| 4 | `GetChatCreateModeAsync` | GET | `/cgi-bin/school/get_chat_create_mode` | 同上；自建 | 92430；`create_mode` 0 自动 / 1 手动 |
| 5 | `SetChatCreateModeAsync` | POST | `/cgi-bin/school/set_chat_create_mode` | 同上；自建 | 92430 |
| 6 | `ConvertToOpenIdAsync` | POST | `/cgi-bin/externalcontact/convert_to_openid` | 同上；自建+第三方+代开发 | 92323/92292/96721；不支持 `wo` 开头的企业微信外部联系人 |
| 7 | `GetAllowScopeAsync` | GET | `/cgi-bin/school/agent/get_allow_scope` | 同上；自建+第三方+代开发 | 94895/94960/96725；`agentid` 走 Query（SCH1b 专项断言） |

### 家校管理配置族（SCH1b，3 支，父接口收敛 + 仅自建/第三方子接口）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 8 | `SetTeacherViewModeAsync` | POST | `/cgi-bin/school/set_teacher_view_mode` | `access_token`(Query)；自建+第三方 | 92652；`view_mode` 1 全部 / 2 仅负责范围 |
| 9 | `GetTeacherViewModeAsync` | GET | `/cgi-bin/school/get_teacher_view_mode` | 同上；自建+第三方 | 92652；官方即 GET |
| 10 | `BatchToExternalUserIdAsync` | POST | `/cgi-bin/externalcontact/batch_to_external_userid` | 同上；自建+第三方 | 92506；单批 ≤100 个手机号 |

### 学生与家长管理族（SCH1c，16 支，父接口收敛 + 三类空标记子接口）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 11 | `CreateStudentAsync` | POST | `/cgi-bin/school/user/create_student` | `access_token`(Query)；自建+第三方+代开发 | 92325/92035/100145；`department` ≤20 |
| 12 | `DeleteStudentAsync` | GET | `/cgi-bin/school/user/delete_student` | 同上；三类 | 92326/92039/100146；官方即 GET，`userid` 走 Query |
| 13 | `UpdateStudentAsync` | POST | `/cgi-bin/school/user/update_student` | 同上；三类 | 92327/92041/100147；`new_student_userid` 仅能改一次 |
| 14 | `BatchCreateStudentAsync` | POST | `/cgi-bin/school/user/batch_create_student` | 同上；三类 | 92328/92037/100148；单批 ≤100 |
| 15 | `BatchDeleteStudentAsync` | POST | `/cgi-bin/school/user/batch_delete_student` | 同上；三类 | 92329/92040/100149；官方批量删除即 POST |
| 16 | `BatchUpdateStudentAsync` | POST | `/cgi-bin/school/user/batch_update_student` | 同上；三类 | 92330/92042/100150 |
| 17 | `CreateParentAsync` | POST | `/cgi-bin/school/user/create_parent` | 同上；三类 | 92331/92077/100151；`children` ≤10 |
| 18 | `DeleteParentAsync` | GET | `/cgi-bin/school/user/delete_parent` | 同上；三类 | 92332/92079/100152；官方即 GET，`userid` 走 Query |
| 19 | `UpdateParentAsync` | POST | `/cgi-bin/school/user/update_parent` | 同上；三类 | 92333/92081/100153；`children` 全量更新 |
| 20 | `BatchCreateParentAsync` | POST | `/cgi-bin/school/user/batch_create_parent` | 同上；三类 | 92334/92078/100154；单批 ≤100 |
| 21 | `BatchDeleteParentAsync` | POST | `/cgi-bin/school/user/batch_delete_parent` | 同上；三类 | 92335/92080/100155；官方批量删除即 POST |
| 22 | `BatchUpdateParentAsync` | POST | `/cgi-bin/school/user/batch_update_parent` | 同上；三类 | 92336/92082/100156 |
| 23 | `GetSchoolUserAsync` | GET | `/cgi-bin/school/user/get` | 同上；三类 | 92337/92038/96738；官方即 GET，`userid` 走 Query |
| 24 | `ListDepartmentStudentsAsync` | GET | `/cgi-bin/school/user/list` | 同上；三类 | 92338/92043/96739；`department_id` 走 Query，无分页 |
| 25 | `ListDepartmentParentsAsync` | GET | `/cgi-bin/school/user/list_parent` | 同上；三类 | 92446/92627/96741；`department_id` 走 Query，无分页 |
| 26 | `SetArchSyncModeAsync` | POST | `/cgi-bin/school/set_arch_sync_mode` | 同上；三类 | 92345/92083/100157；**设置后不可逆** |

### 部门管理族（SCH1e，5 支，父接口收敛 + 三类空标记子接口）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 27 | `CreateDepartmentAsync` | POST | `/cgi-bin/school/department/create` | `access_token`(Query)；自建+第三方+代开发 | 92340/92296/100158 |
| 28 | `UpdateDepartmentAsync` | POST | `/cgi-bin/school/department/update` | 同上；三类 | 92341/92297/100159；管理员逐条 `op` |
| 29 | `DeleteDepartmentAsync` | GET | `/cgi-bin/school/department/delete` | 同上；三类 | 92342/92298/100160；官方即 GET，`id` 走 Query |
| 30 | `ListDepartmentsAsync` | GET | `/cgi-bin/school/department/list` | 同上；三类 | 92343/92299/96745；`id` 可选，不填默认全量 |
| 31 | `SetUpgradeInfoAsync` | POST | `/cgi-bin/school/set_upgrade_info` | 同上；三类 | 92949/92950/100161；`upgrade_time` 只有月日有效 |

### 网页授权登录族（SCH1d，3 支：公共面 1 + 第三方独立 2）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 32 | `GetSchoolUserInfoAsync`（父） | GET | `/cgi-bin/school/getuserinfo` | `access_token`(Query)；自建+代开发 | 95791/96715；`code` 走 Query |
| 33 | `GetUserInfoAsync`（第三方） | GET | `/cgi-bin/service/getuserinfo3rd` | `suite_access_token`(Query)；仅第三方 | 91711；独立路由，不继承父接口 |
| 34 | `GetSchoolUserInfoAsync`（第三方） | GET | `/cgi-bin/service/school/getuserinfo3rd` | `suite_access_token`(Query)；仅第三方 | 95790；独立路由，不继承父接口 |

### 健康上报族（SCH1f，4 支，父接口收敛 + 仅自建子接口）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 35 | `GetHealthReportStatAsync` | POST | `/cgi-bin/health/get_health_report_stat` | `access_token`(Query)；仅自建 | 93676；`date` 最近 30 天；官方即 POST |
| 36 | `GetReportJobIdsAsync` | POST | `/cgi-bin/health/get_report_jobids` | 同上；仅自建 | 93677；`limit` 1~100，默认 100 |
| 37 | `GetReportJobInfoAsync` | POST | `/cgi-bin/health/get_report_job_info` | 同上；仅自建 | 93678；`date` 最近 14 天 |
| 38 | `GetReportAnswerAsync` | POST | `/cgi-bin/health/get_report_answer` | 同上；仅自建 | 93679；`date` 最近 14 天，`limit` ≤100 |

### 上课直播族（SCH1g，5 支，父接口收敛 + 三类空标记子接口）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 39 | `GetLivingInfoAsync` | GET | `/cgi-bin/school/living/get_living_info` | `access_token`(Query)；自建+第三方+代开发 | 93740/93857/97128；`livingid` 走 Query |
| 40 | `GetWatchStatAsync` | POST | `/cgi-bin/school/living/get_watch_stat` | 同上；三类 | 93741/93858/97129；V1，`ending`/`next_key` |
| 41 | `GetUnwatchStatAsync` | POST | `/cgi-bin/school/living/get_unwatch_stat` | 同上；三类 | 93742/93859/97130；V1 |
| 42 | `GetWatchStatV2Async` | POST | `/cgi-bin/school/living/get_watch_stat_v2` | 同上；三类 | 95793/95799/97132；V2，`has_more`/`next_cursor` |
| 43 | `GetUnwatchStatV2Async` | POST | `/cgi-bin/school/living/get_unwatch_stat_v2` | 同上；三类 | 95795/95800/97133；V2 |

### 班级收款族（SCH1h，2 支，父接口收敛 + 仅自建/第三方子接口）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 44 | `GetPaymentResultAsync` | POST | `/cgi-bin/school/get_payment_result` | `access_token`(Query)；自建+第三方 | 94470/94553；仅本应用创建的收款项目 |
| 45 | `GetTradeAsync` | POST | `/cgi-bin/school/get_trade` | 同上；自建+第三方 | 94471/94554；仅本应用创建的订单 |

## 1. 一棵部门树承载「学校 / 校区 / 学段 / 年级 / 班级」——定位键只有 `id` / `parentid` / `type`

家校通讯录不是「学校表 + 学部表 + 年级表 + 班级表」四张独立表，而是**一棵部门树**（[SchoolDepartmentInfo.cs](../../DataModels/School/SchoolDepartmentInfo.cs)）：
校区 / 学段 / 年级 / 班级全部是同一 `department` 集合里的节点，靠 **`type`（`int`）区分层级**：

| `type` | 语义 | 关键约束（接口注释留档） |
|---|---|---|
| 1 | 班级 | 父部门必须是年级（即 `type=1` 的部门的父部门 `type` 必须为 2） |
| 2 | 年级 | 唯一可携带 `standard_grade` / `register_year` 的类型 |
| 3 | 学段 | 学段负责人（管理员 `type=5`）落此 |
| 4 | 校区 | 校区负责人（管理员 `type=1`）落此 |
| 5 | 学校 | 根部门，`id` 固定为 1、`parentid` 为 0（[Department/IWechatWorkSchoolDepartmentService.cs](./Department/IWechatWorkSchoolDepartmentService.cs) 注释） |

**定位键口径**（本仓无 `org_dept_id` / `class_id` / `grade_id` 这些独立键，全仓统一走部门 `id`）：

- 部门增删改：`id`（`int`，创建时指定须 >1、不填自动生成）、`parentid`（`int`）。
- 学生 / 家长按班级拉取：`department_id`（`int`）——即班级节点的 `id`，走 Query（#24 / #25）。
- 学生归属班级：请求体 `department`（`List<int>?`，JSON `department`），单学生 ≤20 个班级。
- 「可使用的家长范围」的部门列表字段名是 **`partyid`、类型 `List<long>?`**（[GetSchoolAllowScopeResponse.cs](../../DataModels/School/GetSchoolAllowScopeResponse.cs)），与部门 `id` 的 `int` **不同宽**，勿合并建模、勿「顺手改成 `int`」。

## 2. 两套身份体系：家校通讯录 userid vs 外部联系人 `external_userid`

| 身份 | 键 | 归属域 | 可见性 |
|---|---|---|---|
| 内部成员（学生） | `student_userid`（`string`，1~64 字节） | 本域 | 全族可读 |
| 内部成员（家长） | `parent_userid`（`string`） | 本域 | 全族可读 |
| 外部联系人（家长微信） | `external_userid`（`string`） | 家校侧读写、语义归**客户联系域** | 仅当家长已关注「学校通知」才返回 |

边界（守卫 SCH 头注「既有覆盖注记」明示，**两路由均不在本模块重复声明**）：

- 发送「学校通知」（`externalcontact/message/send`）由**消息推送域**家校学校通知族承载。
- 获取外部联系人详情（`externalcontact/get`，家校版文档 91670）由**客户联系域**客户管理族承载；其响应中的家校字段 `is_subscribe` / `subscriber_info` 已补入 `ExternalContactInfo`。
- 本域只做转换：`convert_to_openid`（#6，`external_userid`→`openid`，**不支持 `wo` 开头的企业微信外部联系人**）、`batch_to_external_userid`（#10，家长手机号→`external_userid`，关注后才返回）。
- 关键去重口径（[SchoolParentInfo.cs](../../DataModels/School/SchoolParentInfo.cs)）：**对同一个服务商，同一家长微信在不同学校返回的 `external_userid` 相同**——跨校合并家长时以 `external_userid` 归并，不要按学校内 `parent_userid` 归并。

## 3. 同名 / 同证件多学生：唯一键只有 `student_userid` / `parent_userid`

本域**没有身份证 / 证件号字段**——学生报文字段仅 `student_userid` / `name` / `mobile` / `to_invite` / `department`（[SchoolCreateStudentRequest.cs](../../DataModels/School/RequestModel/SchoolCreateStudentRequest.cs)）。
因此：

- **唯一键 = `student_userid` / `parent_userid`**：「学校内必须唯一」；`name` 与 `mobile` **不保证唯一，不得用作定位或去重键**。同名学生靠 `student_userid` 区分。
- `student_userid` 形状（DTO 注释）：1~64 字节、只能由数字、字母和 `_-@.` 四种字符组成、首字符必须是数字或字母、不区分大小写；且**可与本企业通讯录成员 UserID 相同**。
- 变更键：`new_student_userid` / `new_parent_userid` 每个成员**仅能修改一次**，且不得与已存在的 UserID 相同。

## 4. 批量上限、串行、递归——本域不本地拦截

| 场景 | 上限（接口注释 / DTO 留档） | 备注 |
|---|---|---|
| 批量创建 / 更新 / 删除学生、家长 | 每次 ≤ **100** | 部分失败经 `result_list` 逐条返回 |
| 单学生所属班级 | ≤ **20**（`department`） | |
| 单家长孩子列表 | ≤ **10**（`children`） | |
| 手机号转外部联系人 | ≤ **100**（`mobiles`） | #10 |
| 每班级学生总数 | ≤ **3 万** | 官方建议「创建 department 与创建成员**串行化**」 |
| 部门学生 / 家长详情 | **无 cursor/limit 分页** | 取子树须自行「先取子部门、再递归取学生」 |
| 健康上报 | `limit` 1~100；`date` 统计近 30 天、任务详情/答案近 14 天 | #35~#38 |

本仓**不做**本地限流、批次裁剪、递归聚合，越界与超限由应答 `errcode` 表达；递归取子树、串行化提交由宿主负责。

## 5. 「整体替换」与「增量 / 逐条」两类设置端点，勿混用

- **整体替换（全量覆盖）**：更新家长的 `children` 是**全量更新**——官方原文「该字段是全量更新，如果孩子列表为空则忽略该字段」，即**传非空列表 = 全量覆盖、传空数组 = 忽略该字段（不是删除全部孩子关系）**（[SchoolUpdateParentRequest.cs](../../DataModels/School/RequestModel/SchoolUpdateParentRequest.cs)）。`set_teacher_view_mode` / `set_subscribe_mode` / `set_chat_create_mode` / `set_arch_sync_mode` 均为「一次设定全局模式」。
- **增量 / 逐条操作**：更新部门的 `department_admins` 走**逐条 `op` 语义**——`op=0` 新增或更新、`op=1` 删除（[SchoolUpdateDepartmentAdminItem.cs](../../DataModels/School/RequestModel/SchoolUpdateDepartmentAdminItem.cs)）；该条目比创建部门的管理员条目**多一个 `op` 字段、两端点不共用 DTO**（[SchoolDepartmentAdminItem.cs](../../DataModels/School/RequestModel/SchoolDepartmentAdminItem.cs) 无 `op`）。
- **不可逆**：`set_arch_sync_mode` 一旦设置「禁止自动同步」将**无法再次开启**（#26）。

## 6. 查询 / 统计类官方即 POST，删除 / 读取类官方即 GET——勿「顺手统一」

本域「统计/查询」走 POST 是官方契约，不是本仓笔误：健康上报 4 支全 POST、班级收款 2 支全 POST、直播 V1/V2 观看与未观看统计全 POST（SCH1f 断言全部路由位于 `/cgi-bin/health/` 段、SCH1h 断言 2 支即 POST）。
反向的 GET 端点也照官方：#1/#3/#4/#7/#9 二维码与模式读取、#12/#18 单个删除学生/家长、#23/#24/#25 读取与部门列表、#29/#30 删除/列出部门、#32~#34 授权登录、#39 直播详情——**单个删除是 GET（`userid`/`id` 走 Query），批量删除是 POST（`useridlist` 请求体），二者不统一**（SCH1c / SCH1e 专项断言）。

## 7. 字段名照抄官方原文大小写与连写

- 直播观看统计容器字段官方原文是 **`stat_infoes`**（非 `stat_infos`），照抄不纠正（[Living/IWechatWorkSchoolLivingService.cs](./Living/IWechatWorkSchoolLivingService.cs)）。
- 网页授权第三方响应字段是 **PascalCase** 的 `CorpId` / `UserId` / `DeviceId` / `OpenId`，与本域其余 camelCase 字段不同，照抄不纠正（[Auth/IWechatWorkSchoolAuthService_ThirdParty.cs](./Auth/IWechatWorkSchoolAuthService_ThirdParty.cs)）。
- 部门删除 #29 的 `id`：官方参数表标注为「否」，属官方原文，照抄不纠正，但语义上删除必须传目标部门 id（接口注释明示）。
- 连写名 `student_userid` / `parent_userid` / `external_userid` / `register_year` / `standard_grade` / `group_chat_id` / `is_graduated` / `open_group_chat` / `partyid` 一律照官方 JSON 键，C# 属性用 PascalCase、`[JsonPropertyName]` 钉住原文。

## 8. 未成年人个人信息敏感口径

学生 / 家长是**未成年人及其监护人**，其 `name`、`mobile`、身份标识属敏感个人信息，处理须遵循最小必要与合规授权：

- 手机号 `mobile`：第三方应用**不可获取**；代开发应用**需管理员授权手机号权限**才返回（#23/#24/#25 注释）。
- 学生姓名 / 手机号在示例与日志里**一律用假值**（如「张小明」、`+8613800000000`），绝不用真实未成年人数据。
- `access_token` / `suite_access_token` / `code` / `permanent_code` / `auth_code` / `suite_ticket` 只写参数名，**不得进日志、遥测或异常消息**。
- 外部联系人 `external_userid`、`is_subscribe`、`parent_userid` 属可关联到具体个人的标识，落库与出网前须按个人信息保护要求处理。

## 9. 应用类型开放面不等宽（SCH2 层级断言）

| 族 | 自建 | 第三方 | 代开发 | 子接口形态 |
|---|---|---|---|---|
| 基础 / 学生 / 部门 / 直播 | 是 | 是 | 是 | 三类空标记子接口 |
| 管理配置（Setting） | 是 | 是 | **否** | 仅自建 + 第三方（官方未向代开发开放） |
| 健康上报（HealthReport） | 是 | **否** | **否** | 仅自建（第三方/代开发「暂不支持」） |
| 班级收款（ClassPay） | 是 | 是 | **否** | 仅自建 + 第三方（代开发无服务端查询 API） |
| 网页授权（Auth） | 是 | 独立成族 | 是 | 自建/代开发继承父接口；第三方不继承 |

- 空标记子接口 `GetMethods(DeclaredOnly)` 恒为空，全部端点来自父接口继承；自建/第三方/代开发子接口都是**类型化契约入口**，不是端点声明处。
- 网页授权第三方 `IWechatWorkThirdPartySchoolAuthService` **不得**继承父接口（SCH2 反向断言）：父接口以 `access_token` 鉴权，第三方是 `suite_access_token` + `/cgi-bin/service/` 独立路由，继承即令牌路由键漂移。

## 10. 令牌路由键与归属域键（SCH3 + TO 系列）

| 子接口族 | `TokenType` | `TokenManagerKey` | 语义 |
|---|---|---|---|
| `...Internal...`（自建） | `Wechat.AccessToken` | `WechatTokenManagerKeys.InternalAccessToken` | 应用自身 `access_token` |
| `...Provider...` / `...ThirdParty...`（access_token 面） | `Wechat.AccessToken` | `WechatTokenManagerKeys.CorpAccessToken` | 授权企业级令牌，scope = `authCorpId` |
| `IWechatWorkThirdPartySchoolAuthService` | `Wechat.SuiteAccessToken` | （无企业 scope） | 套件级 `suite_access_token` |

`SCH3` 锁定每个接口的 `TokenType` + `TokenInjectionMode.Query` + 参数名（`access_token` 或 `suite_access_token`）。
公共父接口 `IsAbstract=true`、**不声明 `TokenManagerKey`**；归属域仅在应用类型子接口声明，由 `WechatTokenOwnerContractGuards`（TO1~TO3）与 [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs) 锁定。
第三方 / 代开发调用 access_token 面前须经 [`IWechatAppContextSwitcher.UseCorpScope(appKey, authCorpId, permanentCode)`](../../../../../AGENTS.md) 切到目标授权企业作用域（`permanent_code` 只写参数名，不得进日志）。
`suite_access_token` 为套件级凭证、无企业 scope，不经令牌作用域机制表达；其参数名已在组件 `SensitiveUrlRedactor` 词表内（无 G7 豁免负担）。

## 11. 跨域「单一所有者」：两支端点已从本域迁出（SCH1d / SCH1g 断言）

官方以两套文档承载同一端点，本仓收敛为**单一所有者**，本域不再重复声明：

- **获取访问用户身份** `/cgi-bin/auth/getuserinfo`（自建 91707 / 代开发 96712）归**身份验证域** `IWechatWorkIdentityService.GetUserInfoAsync`；家校家长形态的 `parent_userid` 以可空字段并入 `GetUserInfoResponse`。⇒ 网页授权父接口只剩 `school/getuserinfo` **1 支**（SCH2 断言 `authParent` 声明面 `HaveCount(1)`、自建/代开发类型化面各 1）。
- **获取老师直播 ID 列表** `/cgi-bin/living/get_user_all_livingid` 与 **删除直播回放** `/cgi-bin/living/delete_replay_data` 归**直播域** `IWechatWorkLivingService`（独立 `Living` 模块）。⇒ 本域上课直播 5 支**全部位于 `/cgi-bin/school/living/` 段**，SCH1g 专项断言 `/cgi-bin/living/` 段计数为 0、`/cgi-bin/school/living/` 段计数为 5。
- 注意：[Auth/IWechatWorkSchoolAuthService_Internal.cs](./Auth/IWechatWorkSchoolAuthService_Internal.cs) 与 [Auth/IWechatWorkSchoolAuthService_Provider.cs](./Auth/IWechatWorkSchoolAuthService_Provider.cs) 的 XML 仍写「自建与代开发开放的 2 个端点」，属迁出前的**陈旧措辞**，实际类型化面为 1 支，以 SCH2 断言为准（见文末缺口）。

## 12. 接口命名空间按 `IsAbstract` 二分（N1~N3）

- 8 个公共父接口（`IsAbstract = true`）落命名空间 **`Mud.Wechat.Work.Interfaces`**（如 `IWechatWorkSchoolService`、`IWechatWorkSchoolUserService`、`IWechatWorkSchoolAuthService`）。
- 全部可注入子接口（`...Internal...` / `...Provider...` / `...ThirdParty...`）与第三方独立接口落 **`Mud.Wechat.Work`**。
- 父接口 `RegistryGroupName` 恒空；子接口 `RegistryGroupName = "School"`、`InheritedFrom` 指向父接口生成实现类。由 [WechatInterfaceNamespaceContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInterfaceNamespaceContractGuards.cs)（N1~N3）锁定。

## 快速开始

```csharp
// Program.cs：多应用底座先行，再注册本模块
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddSchoolApi());

// 自建应用注入自建子接口（公共父接口 IsAbstract，不可注入）
public sealed class SchoolSync(
    IWechatWorkInternalSchoolUserService users,
    IWechatWorkInternalSchoolDepartmentService departments,
    IWechatWorkInternalSchoolService school)
{
    // 1) 先建年级部门（type=2，班级父部门必须是年级），再建班级（type=1，父为年级 id）
    public async Task<int> EnsureClassAsync(int gradeDeptId, string className, CancellationToken ct)
    {
        var created = await departments.CreateDepartmentAsync(new SchoolCreateDepartmentRequest
        {
            Name = className,              // 示例假值
            Parentid = gradeDeptId,        // int?，官方必填
            Type = 1,                      // 1 班级；父部门 type 必须为 2 年级
        }, ct);
        WechatWorkException.ThrowIfFailed(created, "/cgi-bin/school/department/create");
        return created.Id!.Value;
    }

    // 2) 批量创建学生：单批 ≤100，department ≤20；部分失败逐条读 result_list
    public async Task CreateStudentsAsync(int classDeptId, CancellationToken ct)
    {
        var resp = await users.BatchCreateStudentAsync(new SchoolBatchCreateStudentRequest
        {
            Students =
            [
                new SchoolCreateStudentRequest
                {
                    StudentUserid = "stu_0001",     // 学校内唯一，非姓名/手机号
                    Name = "张小明",                 // 未成年人假值
                    Mobile = "+8613800000000",       // 假值；第三方应用读不到
                    Department = [classDeptId],      // ≤20 个班级
                    ToInvite = true,                 // 仅验证的学校可发起邀请
                },
            ],
        }, ct);
        // 批量端点：errcode 非 0 时仍带 result_list 明细，不能只判 errcode
        List<SchoolStudentBatchResultItem> failed =
            resp.ResultList ?? new List<SchoolStudentBatchResultItem>();
        foreach (SchoolStudentBatchResultItem item in failed)
        {
            // item 的失败学生以 student_userid 定位；据 errcode 记账，勿整体重投
        }
    }

    // 3) 读取部门学生：官方即 GET、department_id 走 Query、无分页；取子树须自行递归
    public async Task<List<SchoolStudentInfo>> ListStudentsAsync(int classDeptId, CancellationToken ct)
    {
        var resp = await users.ListDepartmentStudentsAsync(classDeptId, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/school/user/list");
        return resp.Students ?? new List<SchoolStudentInfo>();
    }

    // 4) 可用家长范围：注意 departments.partyid 是 long，students.userid 是 string
    public async Task ShowAllowScopeAsync(int agentId, CancellationToken ct)
    {
        var resp = await school.GetAllowScopeAsync(agentId, ct);
        WechatWorkException.ThrowIfFailed(resp, "/cgi-bin/school/agent/get_allow_scope");
        SchoolAllowScope? scope = resp.AllowScope;
        List<string> studentIds = scope?.Students?.Userid ?? new List<string>();
        List<long> departmentIds = scope?.Departments?.Partyid ?? new List<long>();
        // studentIds / departmentIds 即「可在学校通知使用该应用」的家长范围
    }
}

// 第三方：先切到授权企业作用域，再用企业级 access_token 调 access_token 面端点
using (switcher.UseCorpScope(appKey: "default", authCorpId, permanentCode))
{
    var one = await thirdPartyUsers.GetSchoolUserAsync("stu_0001", ct);
    WechatWorkException.ThrowIfFailed(one, "/cgi-bin/school/user/get");
    // one.UserType：1 学生（返回 student）/ 2 家长（返回 parent）
}

// 第三方网页授权：suite_access_token 鉴权、无企业 scope，不经 UseCorpScope
var who = await thirdPartySchoolAuth.GetSchoolUserInfoAsync(code, ct);
WechatWorkException.ThrowIfFailed(who, "/cgi-bin/service/school/getuserinfo3rd");
```

判错为单层普通信封（`errcode == 0` 即成功）；批量端点例外——`errcode` 非 0 时仍可能带 `result_list` 逐条明细。令牌失效由基座按 `errcode` 判定并自动恢复，调用方不手写重试。

## 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| SCH4 头注与断言计数不一致 | 守卫 `SchoolDataModels_ShouldBeRegisteredInJsonContext` 的 `<remarks>` 头注写「55 型 / 25 型 / 共 100」，但实际 `HaveCount` 断言为 **School 54 / HealthReport 15 / Living 22 / ClassPay 5 = 96 型**（各因单一所有者收敛迁出 3 型） | 以断言为准（96 型）；头注属陈旧聚合，改动 DTO 登记时以 `HaveCount` 为权威 |
| Auth 自建/代开发子接口「2 个端点」措辞陈旧 | [IWechatWorkSchoolAuthService_Internal.cs](./Auth/IWechatWorkSchoolAuthService_Internal.cs) / [IWechatWorkSchoolAuthService_Provider.cs](./Auth/IWechatWorkSchoolAuthService_Provider.cs) 注释仍写 2 支，实际类型化面 1 支（getuserinfo 已迁「身份验证」域） | 以 SCH2 的 `HaveCount(1)` 为准；补正注释时勿动断言 |
| 本域各端点 QPS 数值 | 接口注释仅留档批量上限（≤100 / ≤20 / ≤10 / ≤3 万）与统计分页（limit 1~100、date 30/14 天），**未留档每秒调用频率** ⇒ 频率数值未逐项核验 | 需要限流时先核验官方页，不得从邻域借数值 |
| `standard_grade` 取值全集 | DTO 与接口注释仅写「详见官方标准年级对照表」，本仓不建枚举、不列取值 ⇒ **未逐项核验** | 建年级校验前先核验官方对照表，再决定是否建常量表 |
| 部门管理员 `type` 与部门 `type` 的完整配对 | 注释给出「校区负责人→校区、班主任/任课老师→班级、年级负责人→年级」示例，但 5 种管理员 type × 5 种部门 type 的合法矩阵**未逐项核验** | 越界由 `errcode` 表达；SDK 不本地拦截 |
| 直播 V2 `has_more` 与 V1 `ending` 的翻页终态 | 注释记 V1 `ending`（1 拉完）、V2 `has_more`（1 还需继续，语义相反），但各自 `next_key`/`next_cursor` 的留存时长与轮询建议间隔**未逐项核验** | 轮询节奏由宿主自定，勿在 SDK 侧加隐式重试 |
| `class_pay` 金额单位 | 本文未写金额字段单位（`amount` 口径）⇒ **未逐项核验**，ClassPay 请求/响应 DTO 未在本篇逐一展开 | 涉及对账前读 [ClassPay/](../../../Mud.Wechat.Work/DataModels/School/ClassPay/) DTO 注释并核验官方页 |

## 事实来源

| 要查什么 | 去哪查 |
|---|---|
| 逐参数原文（含 ≤100 / ≤20 / ≤10 / 3 万、date 30/14 天、全量更新、不可逆、`wo` 开头不支持） | 本目录 28 份接口文件的 XML 注释（按子族落 `Basic/`、`Setting/`、`User/`、`Department/`、`Auth/`、`HealthReport/`、`Living/`、`ClassPay/`） |
| 八族路由 / 计数（7 / 3 / 16 / 5 / 3 / 4 / 5 / 2） | `WechatSchoolContractGuards.cs` SCH1a（`SchoolRoutes`）/ SCH1b（`SchoolSettingRoutes`）/ SCH1c（`SchoolUserRoutes`）/ SCH1e（`SchoolDepartmentRoutes`）/ SCH1d（`SchoolAuthRoutes`）/ SCH1f（`SchoolHealthReportRoutes`）/ SCH1g（`SchoolLivingRoutes`）/ SCH1h（`SchoolClassPayRoutes`） |
| 接口层级 / 空标记 / 第三方不继承父接口 / 类型化面计数 | SCH2 `SchoolInterfaceHierarchy_ShouldMatchOfficialOpenSurfaces` + `CollectInterfaceEndpoints` |
| `access_token` / `suite_access_token` + Query 绑定 | SCH3 `SchoolTokenBinding_ShouldMatchOfficialTokenRouteKeys` |
| DTO 上下文登记（School 54 / HealthReport 15 / Living 22 / ClassPay 5 型） | SCH4 `SchoolDataModels_ShouldBeRegisteredInJsonContext` |
| `agentid` / `userid` / `department_id` / `id` / `code` / `livingid` 的 Query 契约 | SCH1b / SCH1c / SCH1e / SCH1d / SCH1g 内的 `AssertHasQueryParameter` |
| JSON 上下文与命名空间归属（Living 上下文落 `DataModels.Living`） | 守卫头注别名声明 + `../../../Mud.Wechat.Work/DataModels/Generated/` |
| 令牌归属键 TO 系列 / 命名空间分区 N1~N3 | [WechatTokenOwnerContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatTokenOwnerContractGuards.cs)、[WechatInterfaceNamespaceContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatInterfaceNamespaceContractGuards.cs) |
| DTO 属性名 / 类型 / JSON 名 / 取值域注释 | [../../../Mud.Wechat.Work/DataModels/School/](../../../Mud.Wechat.Work/DataModels/School/)（`RequestModel/`、`HealthReport/`、`Living/`、`ClassPay/`） |
| 模块注册三段式 | [WechatModule.cs](../../Extensions/WechatModule.cs)、[WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs)、[WechatJsonResolverExtensions.cs](../../Extensions/WechatJsonResolverExtensions.cs) |
| Query 令牌注入白名单（G5） | [WechatContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatContractGuards.cs) |

## 改动本域需要同批完成

1. 加 / 改端点 → 接口 XML 补齐官方文档号与业务限制（频率 / 上限 / 串行 / 覆盖语义 / 权限可见范围，格式照既有端点，**不得弱化**），**同批**改
   [WechatSchoolContractGuards.cs](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatSchoolContractGuards.cs)
   对应 `SchoolRoutes` / `SchoolUserRoutes` 等八张表与各自 `HaveCount` + `Distinct` 双断言；改开放面须同步改 SCH2 的 `families` 数组与类型化面计数。
2. 加 / 改 DTO → 标 `[HttpJsonSerializable]`（主域 `SerializerClassName = "School"`、子族各自上下文）→ 跑
   `../../../../../scripts/AddHttpJsonSerializable.ps1` 与 `../../../../../scripts/GenerateJsonContext.ps1` →
   更新 SCH4 的四组类型清单与 `HaveCount`（School / HealthReport / Living / ClassPay）。
   注意 `LivingJsonContext` 落在 `DataModels.Living` 命名空间（守卫以别名引用），改名或挪目录须同批改守卫别名。
3. 新增接口族 / 重命名子接口 → 模块三段式同批：[WechatModule.cs](../../Extensions/WechatModule.cs) 的 `School` 枚举成员与 remarks、
   [WechatWorkServiceBuilder.cs](../../Extensions/WechatWorkServiceBuilder.cs) 的 `AddSchoolApi()` XML（其端点叙述须与本域 45 计数一致）、
   可注入子接口 `RegistryGroupName = "School"`（父接口保持空组）。
4. 跨域「单一所有者」边界（`auth/getuserinfo`、`living/get_user_all_livingid`、`living/delete_replay_data`、`externalcontact/message/send`、`externalcontact/get`）
   **不得**在本域重复声明端点；若官方改变承载域，须同时改 SCH1d / SCH1g 的段计数断言与邻域守卫。
5. 凭据面改动一律不得引入真值：`access_token`、`suite_access_token`、`code`、`permanent_code`、`auth_code`、`suite_ticket` 只写参数名；
   学生 / 家长 `name`、`mobile`、`student_userid`、`parent_userid`、`external_userid` 属未成年人个人信息，示例用假值、不得进日志与遥测；新增敏感 Query 参数名须同批登记脱敏词表（G7）。
6. 本文与 [../../README.md](../../README.md) 的域内端点数、官方文档号表述须同步更新；
   最后跑 `../../../../../scripts/verify-build.ps1` 与 `../../../../../scripts/audit-config-keys.ps1`，全绿才算完成。
