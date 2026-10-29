# ExternalContact 域 · 客户联系（外部联系人）

客户联系（外部联系人）是「企业微信 / Work」产品线内规模最大的一域：共 **95 个 HTTP 端点**，按 16 个子族分布于 [`Interfaces/ExternalContact/`](.) 的 16 个子目录。三类应用（企业自建 / 第三方 / 服务商代开发）的公共面端点收敛在 `Mud.Wechat.Work.Interfaces` 命名空间下的 `IsAbstract = true` 父接口，能力差异端点声明在 `Mud.Wechat.Work` 命名空间下的 `IWechatWork{Internal|ThirdParty|Provider}ExternalContact{族}Service` 子接口。

- 16 子族：客户 [`Customer/`](./Customer/IWechatWorkExternalContactCustomerService.cs) 11 · 群发 [`GroupMsg/`](./GroupMsg/IWechatWorkExternalContactGroupMsgService.cs) 11 · 朋友圈 [`Moment/`](./Moment/IWechatWorkExternalContactMomentService.cs) 14 · 联系我 [`ContactWay/`](./ContactWay/IWechatWorkExternalContactContactWayService.cs) 10 · 标签 [`Tag/`](./Tag/IWechatWorkExternalContactTagService.cs) 9 · 获客助手 [`CustomerAcquisition/`](./CustomerAcquisition/IWechatWorkExternalContactCustomerAcquisitionService.cs) 9 · 商品相册 [`ProductAlbum/`](./ProductAlbum/IWechatWorkExternalContactProductAlbumService.cs) 5 · 拦截规则 [`InterceptRule/`](./InterceptRule/IWechatWorkExternalContactInterceptRuleService.cs) 5 · 离职继承 [`ResignedInheritance/`](./ResignedInheritance/IWechatWorkExternalContactResignedInheritanceService.cs) 4 · 客户群 [`GroupChat/`](./GroupChat/IWechatWorkExternalContactGroupChatService.cs) 3 · 在职继承 [`JobInheritance/`](./JobInheritance/IWechatWorkExternalContactJobInheritanceService.cs) 3 · 数据统计 [`Statistics/`](./Statistics/IWechatWorkExternalContactStatisticsService.cs) 3 · 企业服务人员 [`FollowUser/`](./FollowUser/IWechatWorkExternalContactFollowUserService.cs) 3 · 获客组件 [`AcquisitionComponent/`](./AcquisitionComponent/IWechatWorkExternalContactAcquisitionComponentService.cs) 3 · 已服务客户 [`ServedContact/`](./ServedContact/IWechatWorkExternalContactServedContactService.cs) 1 · 附件 [`Attachment/`](./Attachment/IWechatWorkExternalContactAttachmentService.cs) 1。合计 95，与 16 个契约守卫逐族 `HaveCount` 一致（见第 3、7 节）。
- DI 注册组名：`"ExternalContact"`（全部 16 子族共用同一注册组，由源生成器产出 `AddExternalContactWebApiHttpClient()`）。模块枚举 `WechatModule.ExternalContact`，入口 [`AddExternalContactApi()`](../../Extensions/WechatWorkServiceBuilder.cs)。
- 验证日期：2026-10-18（口径以本仓接口 XML、守卫断言、DTO 为准）。

> **事实来源限定**：本节及全篇的端点计数、路由、HTTP 动词、令牌绑定、必填条件与字段拼写，**只来自本仓三类事实来源**——接口方法上的 `[Get]/[Post]/[Token]/[Query]/[Body]/[MultipartForm]` 与 XML 注释、`Tests/**/ContractGuards/` 契约守卫断言、`Mud.Wechat.Work/DataModels/ExternalContact/` 请求/响应 DTO 的 `[JsonPropertyName]` 与注释。官方文档仅作为注释里的 `see href` 出处，**不作为本 SDK 行为的事实**。凡本仓未声明或未逐项核验者，一律进入第 6 节留档缺口，不外推。

## 3. 端点速览

令牌列口径：`令牌 = Wechat.AccessToken`（Query 参数名 `access_token`，MUD005 已知接受风险：官方强制令牌走 Query，无法改用 Header）。「开放面」按承载端点的子接口标注——`三类` 表示父接口收敛端点、三类应用子接口均为空标记可注入；差异端点单列归属。令牌归属：自建 = 应用自身 access_token；第三方 / 代开发 = 授权企业级 access_token（scope = authCorpId，须先经 `IWechatAppContextSwitcher.UseCorpScope` 切换作用域）。

### 3.1 客户 Customer（11，守卫 CU1~CU4）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `GetCustomerListAsync` | GET | `/cgi-bin/externalcontact/list` | AccessToken/三类 | Query `userid`；返回 external_userid 列表，无分页 |
| 2 | `GetCustomerDetailAsync` | GET | `/cgi-bin/externalcontact/get` | AccessToken/三类 | Query `external_userid`、`cursor`（跟进人 >500 才分页）|
| 3 | `BatchGetCustomerDetailsAsync` | POST | `/cgi-bin/externalcontact/batch/get_by_user` | AccessToken/三类 | body cursor+limit（limit ≤100 默认 50）；userid_list ≤100 |
| 4 | `UpdateCustomerRemarkAsync` | POST | `/cgi-bin/externalcontact/remark` | AccessToken/三类 | 可填字段不可同时为空；remark_mobiles 传值即整体覆盖 |
| 5 | `GetCustomerStrategyListAsync` | POST | `/cgi-bin/externalcontact/customer_strategy/list` | AccessToken/三类 | cursor+limit（默认/上限均 1000）|
| 6 | `GetCustomerStrategyDetailAsync` | POST | `/cgi-bin/externalcontact/customer_strategy/get` | AccessToken/三类 | |
| 7 | `GetCustomerStrategyRangeAsync` | POST | `/cgi-bin/externalcontact/customer_strategy/get_range` | AccessToken/三类 | cursor+limit 1000 |
| 8 | `CreateCustomerStrategyAsync` | POST | `/cgi-bin/externalcontact/customer_strategy/create` | AccessToken/三类 | 仅串行调用；子组继承父 privilege（传入被忽略）|
| 9 | `UpdateCustomerStrategyAsync` | POST | `/cgi-bin/externalcontact/customer_strategy/edit` | AccessToken/三类 | admin_list/privilege 传值即整体覆盖 |
| 10 | `DeleteCustomerStrategyAsync` | POST | `/cgi-bin/externalcontact/customer_strategy/del` | AccessToken/三类 | |
| 11 | `ConvertToServiceExternalUserIdAsync` | POST | `/cgi-bin/externalcontact/to_service_external_userid` | AccessToken/仅第三方 | 代开发 external_userid → 第三方 external_userid（95195）|

### 3.2 群发 GroupMsg（11，守卫 GM1~GM4）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `AddGroupMsgTemplateAsync` | POST | `/cgi-bin/externalcontact/add_msg_template` | AccessToken/三类 | 返回 `fail_list`（无许可成员）|
| 2 | `RemindGroupMsgSendAsync` | POST | `/cgi-bin/externalcontact/remind_groupmsg_send` | AccessToken/三类 | |
| 3 | `CancelGroupMsgSendAsync` | POST | `/cgi-bin/externalcontact/cancel_groupmsg_send` | AccessToken/三类 | |
| 4 | `GetGroupMsgListAsync` | POST | `/cgi-bin/externalcontact/get_groupmsg_list_v2` | AccessToken/三类 | v2 路由；cursor+limit |
| 5 | `GetGroupMsgTaskListAsync` | POST | `/cgi-bin/externalcontact/get_groupmsg_task` | AccessToken/三类 | |
| 6 | `GetGroupMsgSendResultAsync` | POST | `/cgi-bin/externalcontact/get_groupmsg_send_result` | AccessToken/三类 | |
| 7 | `SendWelcomeMsgAsync` | POST | `/cgi-bin/externalcontact/send_welcome_msg` | AccessToken/三类 | 入群欢迎语（非模板 CRUD）|
| 8 | `AddGroupWelcomeTemplateAsync` | POST | `/cgi-bin/externalcontact/group_welcome_template/add` | AccessToken/三类 | |
| 9 | `UpdateGroupWelcomeTemplateAsync` | POST | `/cgi-bin/externalcontact/group_welcome_template/edit` | AccessToken/三类 | |
| 10 | `GetGroupWelcomeTemplateAsync` | POST | `/cgi-bin/externalcontact/group_welcome_template/get` | AccessToken/三类 | |
| 11 | `DeleteGroupWelcomeTemplateAsync` | POST | `/cgi-bin/externalcontact/group_welcome_template/del` | AccessToken/三类 | |

### 3.3 朋友圈 Moment（14，守卫 MO1~MO4）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `AddMomentTaskAsync` | POST | `/cgi-bin/externalcontact/add_moment_task` | AccessToken/三类 | 异步：返回 `jobid`；文本/附件不可同时为空；月 10 万次、每分钟 10 次 |
| 2 | `GetMomentTaskResultAsync` | GET | `/cgi-bin/externalcontact/get_moment_task_result` | AccessToken/三类 | **本域唯一 GET 取结果**；Query `jobid` 轮询 |
| 3 | `CancelMomentTaskAsync` | POST | `/cgi-bin/externalcontact/cancel_moment_task` | AccessToken/三类 | |
| 4 | `GetMomentListAsync` | POST | `/cgi-bin/externalcontact/get_moment_list` | AccessToken/三类 | |
| 5 | `GetMomentPublishListAsync` | POST | `/cgi-bin/externalcontact/get_moment_task` | AccessToken/三类 | 方法名与路由不同名（task = 发表列表）|
| 6 | `GetMomentCustomerListAsync` | POST | `/cgi-bin/externalcontact/get_moment_customer_list` | AccessToken/三类 | |
| 7 | `GetMomentSendResultAsync` | POST | `/cgi-bin/externalcontact/get_moment_send_result` | AccessToken/三类 | |
| 8 | `GetMomentCommentsAsync` | POST | `/cgi-bin/externalcontact/get_moment_comments` | AccessToken/三类 | |
| 9 | `GetMomentStrategyListAsync` | POST | `/cgi-bin/externalcontact/moment_strategy/list` | AccessToken/三类 | |
| 10 | `GetMomentStrategyDetailAsync` | POST | `/cgi-bin/externalcontact/moment_strategy/get` | AccessToken/三类 | |
| 11 | `GetMomentStrategyRangeAsync` | POST | `/cgi-bin/externalcontact/moment_strategy/get_range` | AccessToken/三类 | |
| 12 | `CreateMomentStrategyAsync` | POST | `/cgi-bin/externalcontact/moment_strategy/create` | AccessToken/三类 | |
| 13 | `UpdateMomentStrategyAsync` | POST | `/cgi-bin/externalcontact/moment_strategy/edit` | AccessToken/三类 | |
| 14 | `DeleteMomentStrategyAsync` | POST | `/cgi-bin/externalcontact/moment_strategy/del` | AccessToken/三类 | |

### 3.4 联系我 ContactWay（10，守卫 CW1~CW4；全 POST）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `AddContactWayAsync` | POST | `/cgi-bin/externalcontact/add_contact_way` | AccessToken/三类 | 返回 `config_id`；type/scene 必填且条件必填（见第 4.5）|
| 2 | `GetContactWayAsync` | POST | `/cgi-bin/externalcontact/get_contact_way` | AccessToken/三类 | **读取即 POST**，config_id 走 body |
| 3 | `GetContactWayListAsync` | POST | `/cgi-bin/externalcontact/list_contact_way` | AccessToken/三类 | |
| 4 | `UpdateContactWayAsync` | POST | `/cgi-bin/externalcontact/update_contact_way` | AccessToken/三类 | |
| 5 | `DeleteContactWayAsync` | POST | `/cgi-bin/externalcontact/del_contact_way` | AccessToken/三类 | 删除后该 config_id 作废、二维码失效 |
| 6 | `CloseTempChatAsync` | POST | `/cgi-bin/externalcontact/close_temp_chat` | AccessToken/三类 | |
| 7 | `AddGroupChatJoinWayAsync` | POST | `/cgi-bin/externalcontact/groupchat/add_join_way` | AccessToken/三类 | 群入群方式，返回 config_id |
| 8 | `GetGroupChatJoinWayAsync` | POST | `/cgi-bin/externalcontact/groupchat/get_join_way` | AccessToken/三类 | POST 读取 |
| 9 | `UpdateGroupChatJoinWayAsync` | POST | `/cgi-bin/externalcontact/groupchat/update_join_way` | AccessToken/三类 | |
| 10 | `DeleteGroupChatJoinWayAsync` | POST | `/cgi-bin/externalcontact/groupchat/del_join_way` | AccessToken/三类 | |

### 3.5 标签 Tag（9，守卫 CT1~CT4；全 POST）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `GetCorpTagListAsync` | POST | `/cgi-bin/externalcontact/get_corp_tag_list` | AccessToken/三类 | **读取即 POST**；不传 group_id/tag_id 返回全部 |
| 2 | `AddCorpTagAsync` | POST | `/cgi-bin/externalcontact/add_corp_tag` | AccessToken/三类 | 企业标签；同名合并去重 |
| 3 | `EditCorpTagAsync` | POST | `/cgi-bin/externalcontact/edit_corp_tag` | AccessToken/三类 | |
| 4 | `DelCorpTagAsync` | POST | `/cgi-bin/externalcontact/del_corp_tag` | AccessToken/三类 | |
| 5 | `MarkTagAsync` | POST | `/cgi-bin/externalcontact/mark_tag` | AccessToken/三类 | 给客户的**个人/企业标签**打（删）标 |
| 6 | `GetStrategyTagListAsync` | POST | `/cgi-bin/externalcontact/get_strategy_tag_list` | AccessToken/三类 | 规则组标签 |
| 7 | `AddStrategyTagAsync` | POST | `/cgi-bin/externalcontact/add_strategy_tag` | AccessToken/三类 | |
| 8 | `EditStrategyTagAsync` | POST | `/cgi-bin/externalcontact/edit_strategy_tag` | AccessToken/三类 | |
| 9 | `DelStrategyTagAsync` | POST | `/cgi-bin/externalcontact/del_strategy_tag` | AccessToken/三类 | |

### 3.6 获客助手 CustomerAcquisition（9，守卫 CA1~CA4）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `GetAcquisitionLinkListAsync` | POST | `/cgi-bin/externalcontact/customer_acquisition/list_link` | AccessToken/三类 | |
| 2 | `GetAcquisitionLinkAsync` | POST | `/cgi-bin/externalcontact/customer_acquisition/get` | AccessToken/三类 | 按 `link_id` 读取 |
| 3 | `CreateAcquisitionLinkAsync` | POST | `/cgi-bin/externalcontact/customer_acquisition/create_link` | AccessToken/三类 | 返回 link_id |
| 4 | `UpdateAcquisitionLinkAsync` | POST | `/cgi-bin/externalcontact/customer_acquisition/update_link` | AccessToken/三类 | |
| 5 | `DeleteAcquisitionLinkAsync` | POST | `/cgi-bin/externalcontact/customer_acquisition/delete_link` | AccessToken/三类 | |
| 6 | `GetAcquisitionCustomerListAsync` | POST | `/cgi-bin/externalcontact/customer_acquisition/customer` | AccessToken/三类 | cursor+limit |
| 7 | `GetAcquisitionQuotaAsync` | GET | `/cgi-bin/externalcontact/customer_acquisition_quota` | AccessToken/三类 | **本族唯一 GET**，无 body |
| 8 | `GetAcquisitionLinkStatisticAsync` | POST | `/cgi-bin/externalcontact/customer_acquisition/statistic` | AccessToken/三类 | |
| 9 | `GetAcquisitionChatInfoAsync` | POST | `/cgi-bin/externalcontact/customer_acquisition/get_chat_info` | AccessToken/三类 | |

### 3.7 商品相册 ProductAlbum（5，守卫 PA1~PA4；全 POST）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `AddProductAlbumAsync` | POST | `/cgi-bin/externalcontact/add_product_album` | AccessToken/三类 | 返回 product_id |
| 2 | `GetProductAlbumAsync` | POST | `/cgi-bin/externalcontact/get_product_album` | AccessToken/三类 | |
| 3 | `GetProductAlbumListAsync` | POST | `/cgi-bin/externalcontact/get_product_album_list` | AccessToken/三类 | |
| 4 | `UpdateProductAlbumAsync` | POST | `/cgi-bin/externalcontact/update_product_album` | AccessToken/三类 | |
| 5 | `DeleteProductAlbumAsync` | POST | `/cgi-bin/externalcontact/delete_product_album` | AccessToken/三类 | 官方即 `delete_`（非 `del_`）|

### 3.8 拦截规则 InterceptRule（5，守卫 IR1~IR4）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `AddInterceptRuleAsync` | POST | `/cgi-bin/externalcontact/add_intercept_rule` | AccessToken/三类 | |
| 2 | `GetInterceptRuleListAsync` | GET | `/cgi-bin/externalcontact/get_intercept_rule_list` | AccessToken/三类 | **本族唯一 GET** |
| 3 | `GetInterceptRuleAsync` | POST | `/cgi-bin/externalcontact/get_intercept_rule` | AccessToken/三类 | 单条详情走 POST |
| 4 | `UpdateInterceptRuleAsync` | POST | `/cgi-bin/externalcontact/update_intercept_rule` | AccessToken/三类 | |
| 5 | `DeleteInterceptRuleAsync` | POST | `/cgi-bin/externalcontact/del_intercept_rule` | AccessToken/三类 | 官方即 `del_`（非 `delete_`）|

### 3.9 离职继承 ResignedInheritance（4，守卫 RI1~RI4；全 POST）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `GetUnassignedListAsync` | POST | `/cgi-bin/externalcontact/get_unassigned_list` | AccessToken/三类 | cursor+limit |
| 2 | `ResignedTransferCustomerAsync` | POST | `/cgi-bin/externalcontact/resigned/transfer_customer` | AccessToken/三类 | 返回 `customer_resign_result` |
| 3 | `GetResignedTransferResultAsync` | POST | `/cgi-bin/externalcontact/resigned/transfer_result` | AccessToken/三类 | 异步结果轮询 |
| 4 | `ResignedTransferGroupChatAsync` | POST | `/cgi-bin/externalcontact/groupchat/transfer` | AccessToken/三类 | 离职群主转交 |

### 3.10 客户群 GroupChat（3，守卫 GC1~GC4；全 POST）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `GetGroupChatListAsync` | POST | `/cgi-bin/externalcontact/groupchat/list` | AccessToken/三类 | cursor+limit；需 `need_name` |
| 2 | `GetGroupChatDetailAsync` | POST | `/cgi-bin/externalcontact/groupchat/get` | AccessToken/三类 | 详情：owner/notice/member_list |
| 3 | `OpenGidToChatIdAsync` | POST | `/cgi-bin/externalcontact/opengid_to_chatid` | AccessToken/三类 | opengid → chat_id |

### 3.11 在职继承 JobInheritance（3，守卫 JI1~JI4；全 POST）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `TransferCustomerAsync` | POST | `/cgi-bin/externalcontact/transfer_customer` | AccessToken/三类 | 在职客户转接 |
| 2 | `GetTransferResultAsync` | POST | `/cgi-bin/externalcontact/transfer_result` | AccessToken/三类 | 异步结果轮询 |
| 3 | `TransferGroupChatAsync` | POST | `/cgi-bin/externalcontact/groupchat/onjob_transfer` | AccessToken/三类 | 在职群主转交 |

### 3.12 数据统计 Statistics（3，守卫 ST1~ST4；全 POST）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `GetUserBehaviorDataAsync` | POST | `/cgi-bin/externalcontact/get_user_behavior_data` | AccessToken/三类 | day/week/month 粒度，见第 4.6 |
| 2 | `GetGroupChatStatisticAsync` | POST | `/cgi-bin/externalcontact/groupchat/statistic` | AccessToken/三类 | 按群主聚合，offset+limit |
| 3 | `GetGroupChatStatisticGroupByDayAsync` | POST | `/cgi-bin/externalcontact/groupchat/statistic_group_by_day` | AccessToken/三类 | 按天聚合 |

### 3.13 企业服务人员 FollowUser（3，守卫 FU1~FU4）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `GetFollowUserListAsync` | GET | `/cgi-bin/externalcontact/get_follow_user_list` | AccessToken/三类 | 无 body |
| 2 | `GetCustomerAcquisitionPermitAsync` | GET | `/cgi-bin/externalcontact/customer_acquisition_app/get_permit` | AccessToken/仅第三方 | 获客许可（101146）|
| 3 | `CheckFollowUserAsync` | POST | `/cgi-bin/externalcontact/check_follow_user` | AccessToken/仅代开发 | 检查成员是否具客户联系权限（96312）|

### 3.14 获客组件 AcquisitionComponent（3，守卫 AC0~AC4）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `GetAcquisitionComponentAuthInfoAsync` | POST | `/cgi-bin/externalcontact/customer_acquisition/get_comp_auth_info` | AccessToken/仅第三方 | 组件授权信息（99610）|
| 2 | `CreateAcquisitionComponentOnceKeyAsync` | POST | `/cgi-bin/externalcontact/customer_acquisition/create_once_key` | AccessToken/仅第三方 | 一次性 key（99603）|
| 3 | `GetAcquisitionBillListAsync` | POST | `/cgi-bin/service/customer_acquisition/get_bill_list` | **SuiteAccessToken/仅第三方** | 代支付账单（99602）；Query `suite_access_token`，无企业 scope |

### 3.15 已服务客户 ServedContact（1，守卫 SC1~SC4）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `GetServedContactListAsync` | POST | `/cgi-bin/externalcontact/contact_list` | AccessToken/仅自建 | 端点声明在自建子接口（父接口零端点）|

### 3.16 附件 Attachment（1，守卫 UA1~UA4）

| # | 方法 | HTTP 动词 | 路由 | 令牌/开放面 | 备注 |
|---|---|---|---|---|---|
| 1 | `UploadAttachmentAsync` | POST | `/cgi-bin/media/upload_attachment` | AccessToken/三类 | `[MultipartForm] IFormContent`；签名 mediaType/attachmentType/formData；路由挂 `/cgi-bin/media/` 前缀 |

## 4. 不可由方法名推断的契约陷阱

1. **external_userid / unionid 键不对称**：external_userid 是本域各端点的主键；而 `avatar`、`gender`、`unionid`、`remark_mobiles` 等敏感字段第三方 / 代开发**不可获取**（`gender` 不可得时官方返回 0）；`corp_full_name` 仅自建应用返回。`add_way`（添加来源）与 `oper_userid`、`state`、`remark_mobiles` 挂在 `follow_user`/`follow_info` 一层，**不在 `external_contact` 一层**——读取渠道参数须走跟进人结构，不能从客户基本信息取。
2. **unionid → external_userid 转换不属本域**：`/cgi-bin/idconvert/unionid_to_external_userid` 与 `batch/external_userid_to_pending_id` 曾在第三方客户子接口重复声明，现收敛为「账号ID」域 `IWechatWorkAccountIdService` 单一所有者，本域仅保留 `/cgi-bin/externalcontact/to_service_external_userid`（守卫 CU3）。
3. **游标分页 vs 偏移分页两套并存**：列表类端点用 `cursor` + `next_cursor`（判完依据 `next_cursor` 是否为空），limit 上限因族而异（批量客户 100 / 默认 50；规则组 1000）；而数据统计 `GetGroupChatStatisticAsync` 用 `offset` + `limit`（默认 500，1~1000）。二者不可混用游标。
4. **config_id 生命周期（覆盖 vs 追加）**：`add_contact_way` / `add_join_way` 返回的 `config_id` 是长期凭据，`get_contact_way` 为 POST 读取；`update_*` 修改后原二维码仍指向同一 config_id 但内容覆盖；`del_contact_way` 后该 config_id 作废、二维码立即失效，须重新 add。
5. **联系我条件必填**：`type`（1 单人 / 2 多人）与 `scene`（1 小程序 / 2 二维码）必填；`style` 仅 `scene=1` 有效；`user` 在 `type=1` 时须恰好 1 个、`type=2` 时 ≤100；`party` 仅 `type=2`；`is_temp` 仅医疗行业、`mark_source` 仅营销获客场景。
6. **统计粒度与跨度**：`get_user_behavior_data` 按 day/week/month 三档聚合，官方限制查询时间跨度与保留期；`groupchat/statistic` 区间为闭区间、**最大跨度 30 天、数据仅保留 180 天**，时间戳非 0 点自动向下取整到当日 0 点；`owner_filter` 可见范围人数超 1000 报错 81017。
7. **朋友圈异步任务**：`add_moment_task` 不直接返回朋友圈 id，而返回 `jobid`；须用唯一 GET `get_moment_task_result?jobid=...` 轮询结果；`get_moment_task`（POST，方法名 `GetMomentPublishListAsync`）取的是发表列表，与取结果的 `get_moment_task_result` 是两个不同端点。发表范围经 `visible_range.sender_list`（user_list ≤10 万 / department_list）与 `external_contact_list.tag_list`（仅企业标签、不支持规则组标签）表达。
8. **群聊 owner / quit_scene 语义**：详情 `GroupChatDetail.Owner`（JSON `owner`）为群主 userid；成员在 `GroupChatMember` 层，含 `join_scene`（1 直接邀请 / 2 邀请链接 / 3 扫码）、`join_time`、`type`（1 企业成员 / 2 外部联系人）与仅 `need_name=1` 才回填的 `name`；`unionid` 第三方不可得。**本仓 `GroupChatMember` 未建模 quit_scene**（见第 6 节）。
9. **标签同名合并 + 企业/个人两面**：`add_corp_tag` 对同名标签合并去重；标签有两面——企业标签（`*_corp_tag`）与「规则组标签」（`*_strategy_tag`）为两套 CRUD；给客户实际打/删标走独立的 `mark_tag`。
10. **读取即 POST**：`get_contact_way`、`get_corp_tag_list`、`get_groupchat`（list/get）、`get_product_album`、`get_intercept_rule`（单条）、`customer_acquisition/get` 等虽为查询语义，官方即 POST，参数走 body；仅 `get`（客户详情）、`list`（客户列表）、`get_follow_user_list`、`get_moment_task_result`、`get_intercept_rule_list`、`customer_acquisition_quota`、`get_permit` 为 GET。
11. **删除路由拼写不一致**：官方 `delete_product_album`（ProductAlbum 用全称 `delete_`）与 `del_contact_way`/`del_corp_tag`/`del_intercept_rule`/`del_strategy_tag`/`resigned` 系（用缩写 `del_`）并存；本仓方法名统一用 `Delete*`，路由逐字照官方。
12. **令牌例外——获客组件账单**：AcquisitionComponent 的 component 端点走 `Wechat.AccessToken`（Query `access_token`），但 `get_bill_list` 走 `WechatTokenTypes.SuiteAccessToken`（Query `suite_access_token`，套件级、无企业 scope、不切换 corp scope，路由前缀 `/cgi-bin/service/`），由守卫 AC3 断言令牌按 suite/access 分裂。
13. **敏感凭据红线**：`corpsecret`、`permanent_code`、`auth_code`、`suite_ticket`、`provider_access_token` 仅传参数名，**严禁写入日志、遥测、异常消息**；`external_userid` / `unionid` / `openid` / 手机号属敏感标识，示例一律用假值。

## 5. 快速开始

```csharp
using Mud.Wechat.Work.DataModels.ExternalContact.Customer;
using Mud.Wechat.Work;                 // 自建子接口命名空间

// 1) 先注册企业微信应用配置，再按需注册 ExternalContact 模块
builder.Services.AddWechatApp(builder.Configuration, "WechatApps");
builder.Services.AddWechatWorkServices(b => b.AddExternalContactApi());

// 2) 注入自建应用客户子接口（DI 可注入的是子接口，父接口 IsAbstract 不可直接注入）
public sealed class CustomerSyncHandler
{
    private readonly IWechatWorkInternalExternalContactCustomerService _customer;

    public CustomerSyncHandler(IWechatWorkInternalExternalContactCustomerService customer)
    {
        _customer = customer;
    }

    public async Task<IReadOnlyList<string>> PullExternalIdsAsync(string staffUserid, CancellationToken ct)
    {
        // 注意：external_userid 为敏感标识，此处仅演示，勿记录真实值
        GetCustomerListResponse resp = await _customer.GetCustomerListAsync(staffUserid, ct);
        List<string> externalIds = resp.ExternalUserid ?? new List<string>();
        return externalIds;
    }

    public async Task BatchDetailAsync(IReadOnlyList<string> staffUserids, CancellationToken ct)
    {
        // userid_list 最多 100，limit ≤ 100（默认 50），用 cursor 翻页
        BatchGetCustomerDetailsRequest request = new BatchGetCustomerDetailsRequest
        {
            UseridList = staffUserids.Take(100).ToList(),
            Limit = 100,
        };

        string? cursor = null;
        do
        {
            request.Cursor = cursor;
            BatchGetCustomerDetailsResponse resp = await _customer.BatchGetCustomerDetailsAsync(request, ct);
            List<BatchCustomerContactItem> items = resp.ExternalContactList ?? new List<BatchCustomerContactItem>();
            // 处理 items（每项含 external_contact 与 follow_info）……
            cursor = resp.NextCursor;
        }
        while (!string.IsNullOrEmpty(cursor));
    }
}
```

> 第三方 / 代开发应用调用授权企业级端点前，须先经 `IWechatAppContextSwitcher.UseCorpScope` 将令牌作用域切换到目标企业，再注入对应 `IWechatWorkThirdParty…` / `IWechatWorkProvider…` 子接口。示例中的属性名（`UseridList` / `Cursor` / `Limit` / `ExternalContactList` / `NextCursor`）逐字取自 DTO。

## 6. 已知留档缺口

| 缺口 | 现状 | 处置 |
|---|---|---|
| `GroupChatMember.quit_scene` / `quit_time` | 本仓成员 DTO 仅建模 `join_scene`/`join_time`，未见退群字段 | 未逐项核验；如需退群原因请核对官方后补 DTO 与守卫 |
| `ExternalContactInfo.add_way` 归属 | 注释明确 add_way 在 follow_user 层，客户基本信息不重复携带 | 已按仓内注释记录，未逐项核验响应字段全集 |
| 频率限制数字 | 朋友圈 add（月 10 万 / 每分钟 10）取自接口注释；其余族频率未逐端点核验 | 未逐项核验，以官方附录默认值为准 |
| 各 GET/POST 之外的错误码全集 | 仅记录注释中出现过的 730000/81017/60011 等个别码 | 未逐项核验，不在本 README 展开 |

## 7. 事实来源

| 子族 | 接口目录 | 契约守卫 | DTO 目录 |
|---|---|---|---|
| Customer | [`Customer/`](./Customer/) | [CU](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactCustomerContractGuards.cs) | [`DataModels/.../Customer/`](../../../Mud.Wechat.Work/DataModels/ExternalContact/Customer/) |
| GroupMsg | [`GroupMsg/`](./GroupMsg/) | [GM](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactGroupMsgContractGuards.cs) | [`.../GroupMsg/`](../../../Mud.Wechat.Work/DataModels/ExternalContact/GroupMsg/) |
| Moment | [`Moment/`](./Moment/) | [MO](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactMomentContractGuards.cs) | [`.../Moment/`](../../../Mud.Wechat.Work/DataModels/ExternalContact/Moment/) |
| ContactWay | [`ContactWay/`](./ContactWay/) | [CW](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactContactWayContractGuards.cs) | [`.../ContactWay/`](../../../Mud.Wechat.Work/DataModels/ExternalContact/ContactWay/) |
| Tag | [`Tag/`](./Tag/) | [CT](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactTagContractGuards.cs) | [`.../Tag/`](../../../Mud.Wechat.Work/DataModels/ExternalContact/Tag/) |
| CustomerAcquisition | [`CustomerAcquisition/`](./CustomerAcquisition/) | [CA](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactCustomerAcquisitionContractGuards.cs) | [`.../CustomerAcquisition/`](../../../Mud.Wechat.Work/DataModels/ExternalContact/CustomerAcquisition/) |
| ProductAlbum | [`ProductAlbum/`](./ProductAlbum/) | [PA](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactProductAlbumContractGuards.cs) | [`.../ProductAlbum/`](../../../Mud.Wechat.Work/DataModels/ExternalContact/ProductAlbum/) |
| InterceptRule | [`InterceptRule/`](./InterceptRule/) | [IR](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactInterceptRuleContractGuards.cs) | [`.../InterceptRule/`](../../../Mud.Wechat.Work/DataModels/ExternalContact/InterceptRule/) |
| ResignedInheritance | [`ResignedInheritance/`](./ResignedInheritance/) | [RI](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactResignedInheritanceContractGuards.cs) | [`.../ResignedInheritance/`](../../../Mud.Wechat.Work/DataModels/ExternalContact/ResignedInheritance/) |
| GroupChat | [`GroupChat/`](./GroupChat/) | [GC](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactGroupChatContractGuards.cs) | [`.../GroupChat/`](../../../Mud.Wechat.Work/DataModels/ExternalContact/GroupChat/) |
| JobInheritance | [`JobInheritance/`](./JobInheritance/) | [JI](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactJobInheritanceContractGuards.cs) | [`.../JobInheritance/`](../../../Mud.Wechat.Work/DataModels/ExternalContact/JobInheritance/) |
| Statistics | [`Statistics/`](./Statistics/) | [ST](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactStatisticsContractGuards.cs) | [`.../Statistics/`](../../../Mud.Wechat.Work/DataModels/ExternalContact/Statistics/) |
| FollowUser | [`FollowUser/`](./FollowUser/) | [FU](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactFollowUserContractGuards.cs) | [`.../FollowUser/`](../../../Mud.Wechat.Work/DataModels/ExternalContact/FollowUser/) |
| AcquisitionComponent | [`AcquisitionComponent/`](./AcquisitionComponent/) | [AC](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactAcquisitionComponentContractGuards.cs) | [`.../AcquisitionComponent/`](../../../Mud.Wechat.Work/DataModels/ExternalContact/AcquisitionComponent/) |
| ServedContact | [`ServedContact/`](./ServedContact/) | [SC](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactServedContactContractGuards.cs) | [`.../ServedContact/`](../../../Mud.Wechat.Work/DataModels/ExternalContact/ServedContact/) |
| Attachment | [`Attachment/`](./Attachment/) | [UA](../../../../../Tests/Mud.Wechat.Work.Tests/ContractGuards/WechatExternalContactAttachmentContractGuards.cs) | [`.../Attachment/`](../../../Mud.Wechat.Work/DataModels/ExternalContact/Attachment/) |

- DI 入口与模块枚举：[`Extensions/WechatWorkServiceBuilder.cs`](../../Extensions/WechatWorkServiceBuilder.cs)、[`Extensions/WechatModule.cs`](../../Extensions/WechatModule.cs)。
- 产品线 README：[`../../README.md`](../../README.md)。
- 客户详情字段口径 DTO：[`ExternalContactInfo.cs`](../../DataModels/ExternalContact/Customer/ExternalContactInfo.cs)、[`CustomerFollowUser.cs`](../../DataModels/ExternalContact/Customer/CustomerFollowUser.cs)。

## 8. 改动本域需要同批完成

1. **接口 + 守卫**：新增/删除/改动词或路由端点时，同步更新对应族守卫的 `Routes` 数组、`HaveCount`、`Distinct` 断言、接口层级（父 `IsAbstract` + 子标记）与令牌绑定（`TokenType` / `InjectionMode.Query` / `Name`）断言；AcquisitionComponent 改动须同时维护 `AC0`（不得重复声明 customer_acquisition 路由）与 `AC3`（suite/access 令牌分裂）。
2. **DTO + 序列化**：新增请求/响应类型须带 `[HttpJsonSerializable(SerializerClassName = "<族>")]` 与逐字 `[JsonPropertyName]`，并确认族内 `*JsonContext` 已登记（守卫 DataModels 注册断言）；`[MultipartForm]` 端点走 `IFormContent` 而非 `[FormContent]`。
3. **WechatModule + 入口**：新增子族沿用 `WechatModule.ExternalContact` 与注册组 `"ExternalContact"`；如需新注册组则同时补 `WechatWorkServiceBuilder` 的 `Add…Api()` 与 `InitializeRegistrars`，并跑源生成器产出 `Add<组>WebApiHttpClient()`。
4. **回调事件**：本域的事件推送（如客户添加/删除、群变更回调）payload 建模属 **Callback 包**，本域只声明 HTTP 端点；改事件键须去 Callback 包同批处理，勿在此登记。
5. **敏感信息红线**：任何改动不得把真实令牌 / `permanent_code` / `suite_ticket` / `provider_access_token` 等写入示例、日志或异常；示例用假 external_userid/unionid。
6. **本 README**：端点计数、路由、动词、令牌、开放面变化须回写本文件第 3、4 节，并复核第 6 节缺口与相对链接（跑链接校验，零坏链）。
