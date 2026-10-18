// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.Extensions;

/// <summary>
/// 企业微信 API 模块枚举（对齐 FeishuModule）。
/// </summary>
/// <remarks>
/// 令牌签发客户端（gettoken / get_provider_token / get_suite_token / get_corp_token）
/// 位于 Abstractions 的 Authentication 注册组，随令牌底座自动注册，不经本枚举。
/// 枚举成员按实际业务使用热度排序（高频模块在前）。
/// </remarks>
public enum WechatModule
{
    /// <summary>客户联系（企业服务人员管理域 + 客户管理域 + 客户标签管理域 + 在职继承域：公共面 + 第三方/代开发能力差异端点）。</summary>
    ExternalContact,

    /// <summary>消息推送（发送应用消息族：三类应用公共面收敛父接口 + 第三方 template_msg 差异端点；群聊会话族 / 智能表格群聊族官方仅自建；家校学校通知族：三类应用公共面收敛父接口 + 空标记子接口）。</summary>
    Message,

    /// <summary>通讯录（成员/部门/标签/通讯录查看权限/异步导入/异步导出六域）。</summary>
    Contact,

    /// <summary>审批（审批申请数据族：三类应用公共面收敛父接口 + 差异端点子接口 —— 获取审批数据（旧）官方仅自建；审批模板域：获取模板详情三类公共，创建/更新模板自建与代开发开放、复制/更新模板到企业官方仅第三方；假期管理域 + 审批流程引擎域：三类应用公共面收敛父接口 + 空标记子接口）。</summary>
    Approval,

    /// <summary>素材管理（临时素材上传/获取 + 上传图片 + 高清语音素材 + 异步上传临时素材：三类应用公共面收敛父接口 + 空标记子接口；服务商上传临时素材：官方仅第三方开放，走 provider_access_token 独立成族）。</summary>
    Media,

    /// <summary>身份验证（网页授权登录/企业微信Web登录身份获取域 + 二次验证域：自建/代开发公共面收敛父接口；第三方身份获取走套件令牌独立成族）。</summary>
    Identity,

    /// <summary>JS-SDK（获取企业 jsapi_ticket + 获取应用 jsapi_ticket 两端点三类应用公共面收敛父接口 + 空标记子接口，官方三份文档逐字一致；ticket/get 端点以固定 Query 值 type=agent_config 携带官方鉴权类型；JS-SDK 签名算法为页面/服务器端 SHA-1 约定，不在 HTTP 端点面内）。</summary>
    JsSdk,

    /// <summary>应用管理（获取应用族：agent/get + agent/list 两端点三类应用公共面收敛父接口 + 空标记子接口，设置应用官方仅企业可调用——第三方以及代开发自建应用不可调用、落自建差异端点；工作台自定义展示族 5 端点三类应用公共面收敛父接口 + 空标记子接口；自定义菜单族 3 端点官方仅自建应用开放——官方权限为「仅企业可调用；第三方不可调用」且代开发章节无对应 API，零端点父接口 + 仅自建子接口承载；自建应用迁移成代开发应用族官方仅代开发章节提供但消费待迁移自建应用自身 access_token，落 Internal 归属域子接口、suite_access_token 为官方包体参数经请求体显式传入）。</summary>
    Agent,

    /// <summary>授权流接口（get_pre_auth_code / set_session_info / get_permanent_code / get_auth_info）。</summary>
    Authentication,

    /// <summary>基础接口（获取企业微信接口IP段 + 获取企业微信回调IP段 2 端点为自建/代开发公共面收敛父接口 + 空标记子接口，官方权限说明均为「无限定」；官方第三方应用开发文档树无「基础接口」分组，不设第三方子接口；两端点官方即 GET、无请求体）。</summary>
    Basic,

    /// <summary>打卡（打卡规则族：获取员工打卡规则三类公共，获取企业所有打卡规则与管理打卡规则 4 写端点自建/代开发开放、第三方暂不支持；打卡记录族：获取打卡记录数据三类开放但第三方为旧字段结构、补卡/添加打卡记录/录入人脸官方仅自建；打卡报表族：日报/月报三类开放但第三方为旧字段结构；打卡排班族 + 设备打卡数据族：三类应用公共面收敛父接口 + 空标记子接口；设备打卡数据路由挂 /cgi-bin/hardware/ 域）。</summary>
    Checkin,

    /// <summary>会议（预约会议基础管理族：创建/修改/取消/获取成员会议 ID 列表 4 端点为三类应用公共面收敛父接口；获取会议详情为自建/第三方差异端点、代开发零端点空标记；会议统计管理族：获取会议发起记录官方仅自建开放，零端点父接口 + 仅自建子接口承载）。</summary>
    Meeting,

    /// <summary>日程（管理日历族：创建/更新/获取/删除日历 4 端点为三类应用公共面收敛父接口 + 空标记子接口；官方契约陷阱：创建日历路由为 calendar/add 而非 create，更新操作为覆盖式而非增量式，获取日历响应管理员字段官方示例作 adminis）。</summary>
    Schedule,

    /// <summary>文档（管理文档族 + 管理文档内容族 + 管理表格内容族 + 管理智能表格内容族：三类应用公共面收敛父接口 + 空标记子接口；编辑文档内容与编辑表格内容为批量更新形态，单次操作数量官方分别限制 30 与 5；管理智能表格内容族为 20 个端点，子表/视图/字段/记录/编组各 4 个，单表上限官方分别限制 200 视图 / 150 字段 / 150 编组 / 100000 行）。</summary>
    Wedoc,

    /// <summary>微盘（管理空间族 4 端点 + 管理空间权限族 5 端点 + 管理文件族 11 端点 + 管理文件权限族 6 端点 + 版本和容量管理族 2 端点，均为三类应用公共面收敛父接口 + 空标记子接口；高级功能账号管理族 3 端点官方仅自建开放，零端点父接口 + 仅自建子接口承载、路由挂 vip/ 段；官方契约陷阱：获取空间信息存在新旧两条路由——旧版 space_info 挂「管理空间」分组、新版 new_space_info 含 secure_setting 挂「管理空间权限」分组；文件分块上传与版本容量均为一页多路由）。</summary>
    Wedrive,

    /// <summary>账号ID（ID 转换 + tmp_external_userid 转换 + 自建应用对接 + corpid 转换 + ID 迁移完成状态 + 智能机器人 userid 转换 + 群 ID 升级，跨 access/provider/suite 三种令牌路由键七接口族）。</summary>
    AccountId,

    /// <summary>微信客服（客服账号管理域 + 接待人员管理域：三类应用公共面收敛父接口 + 空标记子接口）。</summary>
    Kf,

    /// <summary>邮件（应用邮箱侧——发送邮件族：三类应用公共面收敛父接口 + 空标记子接口，普通/日程/会议三端点共用 compose_send 路由；获取接收的邮件族：三类应用公共面收敛父接口 + 空标记子接口；管理应用邮箱账号族：官方仅自建开放，零端点父接口 + 仅自建子接口承载。管理端侧——管理邮件群组/管理公共邮箱/高级功能账号/成员邮箱操作/其他邮件客户端登录设置五族：官方均仅自建开放，零端点父接口 + 仅自建子接口承载）。</summary>
    Mail,

    /// <summary>企业支付（对外收款记录域：三类应用公共面收敛父接口 + 空标记子接口；收款商户号管理域 + 资金流水域 + 创建对外收款账户域 + 普通支付域 + 退款域 + 交易账单域：官方仅自建，零端点父接口 + 仅自建子接口承载端点）。</summary>
    Pay,

    /// <summary>安全管理（文件防泄漏 / 设备管理 / 截屏录屏管理 / 域名 IP 信息 / 高级功能账号管理 / 操作日志，官方仅向自建应用开放）。</summary>
    Security,

    /// <summary>上下游（基础接口 + 关联客户信息 + 上下游通讯录管理：应用共享信息、下级企业凭证、小程序 session、unionid 转换、通讯录导入/查询）。</summary>
    CorpGroup,

    /// <summary>家校沟通（家校沟通基础域：三类应用公共面收敛父接口 + 空标记子接口；家校管理配置域：官方仅自建与第三方开放，不设代开发子接口）。</summary>
    School,

    /// <summary>直播（创建/修改/取消预约直播 + 删除直播回放 + 获取微信观看直播凭证 + 获取成员直播 ID 列表 + 获取直播详情 + 获取直播观看明细 + 获取跳转小程序商城的直播观众信息，9 端点为三类应用公共面收敛父接口 + 空标记子接口；官方契约陷阱：获取直播详情官方即 GET（livingid 走 Query）、其余官方即 POST；获取成员直播 ID 列表与删除直播回放两条路由与家校沟通·上课直播域共用（官方同一端点分文档）；获取成员直播 ID 列表以 next_cursor 分页、获取直播观看明细以 next_key + ending 分页且拉完判定只能依据 ending）。</summary>
    Living,

    /// <summary>数据与智能专区（基础接口域：三类应用公共面收敛父接口 + 差异端点子接口 —— 获取授权信息官方不支持自建、文档存档授权信息官方仅第三方；专区调试模式开启/关闭/查询状态三端点为三类应用公共面；应用调用专区程序域：同步/异步调用三类应用公共面收敛父接口 + 空标记子接口。官方「数据与智能专区」章节无「数据与智能功能族」HTTP 分组——获取会话记录实为专区程序调用 SDK 的 sync_msg 专区面、获取消息统计官方无文档页，两端点经证伪已回退删除；「会话存档数据面」19 条 /cgi-bin/chatdata/* 路径（含已下线 specapi 两条）同属专区程序调用 SDK 面——官方页只有「请求接口名」无请求地址，曾用 HTTP 文档页已从官方目录移除，网关残留注册不构成契约，由守卫 DZ5 断言零声明，证据链见 .docs 定夺三、专区会话存档数据面路由核验留档与 WechatDataZoneContractGuards remarks）。</summary>
    DataZone,

    /// <summary>会话内容存档（开启成员列表域 + 机器人信息域 + 会话同意情况域 + 内部群信息域：官方仅自建，零端点父接口 + 仅自建子接口承载端点；access_token 须由会话内容存档应用 secret 获取。91774 的原生 C SDK 拉取/解密面不在 HTTP SDK 范畴）。</summary>
    MsgAudit,

    /// <summary>电子发票（查询电子发票 + 更新发票状态 + 批量更新发票状态 + 批量查询电子发票：三类应用公共面收敛父接口 + 空标记子接口）。</summary>
    Invoice,

    /// <summary>政民沟通（配置网格结构域：自建/代开发公共面收敛父接口 + 空标记子接口；获取网格列表官方仅自建开放，独立成族不设代开发子接口；配置事件类别域：自建/代开发公共面收敛父接口 + 空标记子接口；巡查上报族 + 居民上报族：官方仅自建开放，零端点父接口 + 仅自建子接口承载端点；第三方应用官方对全部端点标注暂不支持，不设第三方子接口）。</summary>
    Gov,

    /// <summary>紧急通知（发起语音电话 + 获取接听状态：官方仅自建开放，零端点父接口 + 仅自建子接口承载端点；代开发文档页与自建页逐字一致但权限表标注「暂不支持」，不设代开发/第三方子接口）。</summary>
    Emergency,

    /// <summary>推广二维码（官方仅第三方应用开发开放，两族两令牌：企业注册族「获取注册码 + 查询注册状态」走 provider_access_token，零端点父接口 + 唯一第三方子接口承载；通讯录迁移族「设置授权应用可见范围 + 设置通讯录同步完成」消费查询注册状态返回的通讯录迁移 access_token，以显式 Query 传令牌、不带 [Token]、无父接口与子接口；查询注册状态仅支持 24 小时内且仅限注册完成回调事件或获取注册码返回的 register_code，非全新创建企业返回 84024；可见范围三参数未填即清空）。</summary>
    PromotionQrCode,

    /// <summary>收银台（官方在第三方应用开发与服务商代开发两棵文档树的「收银台」分组下提供端点、共享同一端点页（第三方树 98045/98046/98053/98054/99436/99437 = 代开发树 99358/99359/99360/99361/99447/99448，签名算法 98768 = 99362），企业自建应用开发文档树无对应 API；三族两令牌：收款工具族 4 端点与发票管理族 2 端点走 provider_access_token，各为「零端点父接口 + 唯一第三方子接口承载」；应用版本付费族 3 端点走 suite_access_token，为「零端点父接口 + 唯一第三方子接口承载」；官方契约要点：收款工具族须携带 nonce_str/ts/sig 签名三要素（HMAC-SHA256 + Base64），发票管理与应用版本付费族不签名；应用版本付费的「获取企业永久授权码 / 获取企业授权信息」与授权流族为同一端点、已在该族承载故不重复声明；收款订单详情官方同时列出 pay_from 与 pay_type 而示例只用 pay_type；应用版本变更通知官方键值拼写为 change_editon（少一个字母 i）；回调事件 open_order/change_order/pay_for_app_success/refund/change_editon/cancel_order 经指令回调 URL 以套件信封推送）。</summary>
    PayTool,

    /// <summary>智能机器人（官方文档树整体位于「企业自建应用开发」分类下，正文零提及第三方 / 服务商代开发，凭证在企业侧后台配置 ⇒ 零端点父接口 + 唯一自建子接口承载端点：主动回复消息 1 端点，以 URL 一次性凭据 response_code 鉴权、不带 [Token]；回调接收与被动回复走 Callback 包的 JSON 通道（不并入 XML 事件信封）；长连接为可选后续里程碑）。</summary>
    Aibot,

    /// <summary>接口调用许可（官方在第三方应用开发与服务商代开发两棵文档树的「接口调用许可」分组下提供端点、共享同一端点页，企业自建应用开发文档树无对应 API，且该分组无对应事件回调；四族 25 端点统一走 provider_access_token，各为「零端点父接口 + 唯一第三方子接口承载」：订单管理族 13 端点（下单购买 97182 / 下单续期与多企业新购任务提交 97183+98887 / 订单列表与详情 97184+97185 / 订单账号列表 97186 / 取消订单 97187 / 多企业订单详情 98888 / 余额支付 99420）、账号管理族 9 端点（激活 97188 / 激活码详情 97189 / 账号列表 97190 / 成员激活详情 97191 / 账号继承 97192 / 分配激活码 97193）、应用管理族 1 端点（97194）、自动激活设置族 2 端点（97199+97200）；官方契约陷阱：路由 list_actived_account 官方拼写少一个 i、试用期字段名官方拼写为 trail_info、余额支付结果顶层 errcode 表示接口调用成功而非支付成功）。</summary>
    License,

    /// <summary>群机器人 Webhook 推送（官方「服务端API → 群机器人」：发送消息 8 种 msgtype 逐类型一方法同路由 /cgi-bin/webhook/send + 上传媒体文件 /cgi-bin/webhook/upload_media 共 9 端点，官方文档 91770；端点以 URL Query 上的 key 为机器人凭据、不带 [Token]（第三个无令牌例外，见 TO1）；机器人由用户在群聊设置中添加、凭据由官方直接颁发，与应用令牌体系无关 ⇒ 零端点父接口 + 唯一自建子接口承载端点，无第三方/代开发子接口；频率限制每个机器人 20 条/分钟；key 为长期有效凭据且参数名不在组件静态词表内，模块注册期经组件公开登记门面 SensitiveUrlKeys.Register("key") 登记为进程级强制掩码键（见守卫 WEB3））。</summary>
    Webhook,

    /// <summary>人事助手（官方「人事助手 → 花名册」分组：获取员工字段配置 99131 + 获取员工花名册信息 99132 + 更新员工花名册信息 99133 共 3 端点，官方仅自建开放——代开发与第三方权限表均标注暂不支持，零端点父接口 + 仅自建子接口承载端点；官方文档陷阱：get_fields 的字段信息说明表记作 field_id 而请求/响应示例均为 fieldid，本仓以示例为准；update_staff_info 的 update_items / remove_items / insert_items 三者不能全部为空，官方标注不支持更新的字段为 11006/11012/12004）。</summary>
    Hr,

    /// <summary>公费电话（获取公费电话拨打记录 93662 单端点，官方仅自建开放——代开发与第三方权限表均标注暂不支持，零端点父接口 + 仅自建子接口承载端点；查询范围为 [start_time, end_time] 双闭区间、最大跨度 30 天，越界回 600018；与「紧急通知应用」域的 pstncc 路由族分属官方两棵章节树。健康上报 4 端点官方章节即「家校应用 → 健康上报」，已由 School 模块 HealthReport 族承载，勿在此重复声明）。</summary>
    Dial,

    /// <summary>会议室（官方「会议室」文档树只在企业自建应用开发根下存在页面（分类 93617：93619 会议室管理 / 93620 会议室预定管理），第三方与代开发两棵树均无对应页面 ⇒ 两族各为零端点父接口 + 仅自建子接口承载端点：会议室管理族 4 端点（add / list / edit / del）+ 会议室预定管理族 6 端点（get_booking_info / book / book_by_schedule / book_by_meeting / cancel_book / bookinfo/get），共 10 端点且官方全部为 POST；93620 现行正文锚点仅六节，第三方 SDK 转述的 get_booking_info_by_meeting_id 无官方出处 ⇒ 零声明；官方契约陷阱：book_by_meeting 的会议 ID 字段官方拼写为 meetingid（无下划线）、排期容器名 schedule 在 get_booking_info 是数组而在 bookinfo/get 是对象、cancel_book 的定位键官方为 booking_id 而非会议 ID、edit / del / cancel_book 三端点成功响应无业务字段；与「视频会议」域的硬件会议室（/cgi-bin/meeting/rooms/*）分属官方两棵章节树，勿混淆）。</summary>
    MeetingRoom,

    /// <summary>汇报（日志）（官方「汇报」文档树只在企业自建应用开发根下存在页面（分类 93392：93393 获取汇报记录号列表 / 93394 获取汇报记录详情 / 93395 获取汇报统计数据）⇒ 零端点父接口 + 仅自建子接口承载 3 端点，官方全部为 POST 且权限口径为「配置到『汇报 - 可调用接口的应用』中」、代开发与第三方官方均标「暂不支持」；官方频率：93393 独立给出「一次最多拉取 100 个汇报记录」与「调用频率不可超过 600 次/分」，另两页无独立数字阈值走官方附录默认值；官方契约陷阱：93393 的 cursor / limit 与 93395 的 template_id 官方参数表均为必填（template_id 必填即本接口按单一表单取统计、无可选参数），endflag 与迟交标识 flag 官方以数字 0/1 承载、评论 commentid / tocommentid 官方线格式为数字，本仓无数值转换器故分别以 int / long 原样承载；统计侧部门与标签字段名为 open_partyid / open_tagid（非 partyid / tagid）；「汇报」模板是特殊模板，其表单不在 apply_data 中返回而落 info.sys_journal_data（富文本）；汇报内容 apply_data 与审批的 apply_data 同为一棵「控件 + 控件值」树，本域按汇报详情的 value 附录建模（不含审批侧 new_tips / bank_account，另含 students / classes / docs / wedrive_files）；与「审批」域分属官方 OA 的两棵子树，汇报导出两页（96108）不在应用开发目录树内、本域零声明）。</summary>
    Journal,

    /// <summary>智能设备数据（官方「智慧硬件服务端」文档树只在第三方应用开发根下存在页面（分类 96107 → 接口调用）⇒ 零端点父接口 + 唯一第三方子接口承载 8 端点，官方全部为 POST：数据获取族 4 端点（get_auth_info 96097 / get_checkin_data 96027 / get_temperature_data 96028 / get_accesscontrol_data 96029）+ 门禁规则管理族 4 端点（get_accesscontrol_rule 96030 / add_accesscontrol_rule 96031 / mod_accesscontrol_rule 96221 / del_accesscontrol_rule 96227）；官方契约陷阱：设备定位键 get_accesscontrol_rule 用单值 device_sn 而添加 / 修改用 device_sn_list、pass_rule 在查询响应是「容器 + items」而在添加 / 修改请求是单对象、规则名请求侧 rule_name 响应侧 name、设备列表容器内层元素名 item（单数）而三条数据面容器内层元素名 items、三条数据面请求字段面同构而响应容器名分别为 checkindata（连写）/ temperature_data / accesscontrol_data、条目时间字段名 checkin_time 与 timestamp、温度值官方以字符串承载；成员标识全线为 open_userid；与「智能硬件开发」根树的 /cgi-bin/openhw/*（凭据链 model_token / device_token，不属三类应用令牌面）分属两棵章节树）。</summary>
    DeviceData,

    /// <summary>开放硬件（官方「智能硬件开发 → 硬件云端接入」根树：<c>/cgi-bin/openhw/*</c> 17 端点，官方全部为 POST；本域不属三类应用令牌面而是自成两级凭证（<c>model_access_token</c> 由 model_id + model_secret + 推送的 model_ticket 换取 95992；<c>device_access_token</c> 由 device_sn + device_secret 换取 96022），故为「无父接口、无应用类型子接口的独立端点接口 + 显式 Query 传凭证、不带 [Token]」形态（对齐 PromotionQrCode 通讯录迁移族与 ProviderAuthenticationUrl）；凭证层级为<b>逐页结论</b>而非路由前缀推论：型号面 6 支（get_device_secret 95993 / get_device_token 96022 / model/register_sn 95980 / model/unregister_sn 95981 / device/get_device_detail 95982 / device/gen_id_dynamic_qrcode 98024）走 model_access_token，设备面 10 支走 device_access_token，get_model_token 95992 无凭证参数；官方契约陷阱：model/register_sn 应答字段名为 qr_code 而二维码族为 qrcode_content、get_userinfo_by_page 返回 next_cursor + perm_version 而 get_userinfo_by_ids 二者皆无、only_perm_version=true 时官方忽略其余入参、批量上报三支单次上限 100 条且门禁/打卡两族失败项只有定位三键而体温族逐条携带 errcode + errmsg、request 侧 errcode 在固件升级 / 识别信息 / 远程开门三支是「设备侧执行结果」与响应返回码同名不同义、体温值官方为 string、auth_code 仅支持消费一次；<c>model_ticket</c> 经官方推送事件（96053）到达，属接收面不在本 HTTP 面；旧 <c>service/*</c> 硬件族（90626）与 <c>hardware/get_device_feature</c>（92739）已脱树或正文清空 ⇒ 零声明；与「智能设备数据」（/cgi-bin/externalcontact 之外的 device 打卡数据族，走第三方令牌）分属两棵章节树）。</summary>
    OpenHardware,

    /// <summary>代开发应用授权企业的数据接口（官方「服务商代开发 → 应用接口调用」根树的 <c>/cgi-bin/service/*</c> 族：授权企业通讯录检索 4 端点（search / batchsearch / id_translate / sort，97362 系列）+ 异步任务结果查询 1 端点（service/batch/getresult，GET）共 5 端点；全部走 <c>provider_access_token</c>（服务商级令牌）且请求体首字段恒为被授权企业 <c>auth_corpid</c> ⇒ 本域零自建子接口、只有服务商形态子接口，注册组名统一为 Service（令牌类型同族，不得与通讯录 Contact 组混挂）；官方契约陷阱：关键字长度上限 64 字节、limit 50/200 在「在职 / 离职」两桶间分配、检索范围受 agentid 可见范围约束、batchsearch 单次 ≤50 条子请求且<b>逐条</b>返回 errcode/errmsg（顶层 errcode 为 0 不代表子请求全成功）、id_translate 输入为 media_id 列表且输出格式官方仅 pdf、sort 的 useridlist 上限 1000 且 sort_options 为优先级有序数组；service/batch/getresult 返回的 url 携带成员身份且 24 小时有效，不得落日志；SKIT 清单内其余 10 条 <c>service/*</c> 行已核定为非契约（脱树/正文清空），由 RD1 零声明守卫锁定）。</summary>
    Service,

    /// <summary>高级接口订单与代付（官方「服务商代开发 → 高级接口」根树：<c>/cgi-bin/advanced_api/*</c> 6 端点，官方<b>全部为 POST</b> 且统一走 <c>provider_access_token</c>；预订单/取消订单/提交支付 3 支写端点 + 订单列表/订单详情/企业购买信息 3 支查询端点；官方错误码族 701140/701141/701142/701143/701146/701149/701163/701164 与 84077、90432/90433/48001/701084/701147；官方契约陷阱：<c>has_more</c> 本域为整型 0/1（高级功能申请单域为布尔，勿归一）、订单详情的 <c>custom_corpid</c> 返回<b>加密后</b> corpid、聊史高级接口的版本周期信息为 <c>begin_time</c>/<c>end_time</c> 双段数组；<b>高危不可逆写面</b>：CreateOrder 产生真实订单、SubmitOrderPay 提交后官方明确「无法再变更支付方式」，两支均无自动重试；<c>payer_userid</c> 为成员身份不得进日志）。</summary>
    AdvancedApi,

    /// <summary>高级功能成员申请单（官方「高级功能 → 成员申请管理」根树：<c>/cgi-bin/advanced_feature/*</c> 3 端点（set_approval_detail 99880 / get_apply_id_list 99883 / get_approval_info 99885），官方<b>全部为 POST</b>；三页应用类型表均为「自建应用：配置到『高级功能 - 成员申请管理 - 通过 api 设置申请流程』中；代开发与第三方：暂不支持」⇒ 零端点差异、仅唯一自建子接口承载，第三方/代开发形态由本域守卫锁定零回潮；官方契约陷阱：<b>两套 approval_status 值域不同名同义</b>（设置面 1/2/3/101，查询面 0/1/2/3/4/5 多了管理员驳回与管理员已分配），本域 <c>has_more</c> 为布尔而高级接口订单域为整型，limit 默认 100/最大 200 且官方不保证返回条数等于 limit；<b>覆盖式全量写入</b>：官方明确变更审批节点须全量传入，只传部分等于删除其余节点，审批 id 与申请 id 一一对应不可改变；<c>request_reason</c> 为成员自由文本、审批人列表为成员身份，均不得进日志）。</summary>
    AdvancedFeature,
}