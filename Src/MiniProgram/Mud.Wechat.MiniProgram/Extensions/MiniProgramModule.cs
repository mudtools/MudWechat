// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.Extensions;

/// <summary>
/// 微信小程序 API 模块枚举（对齐公众号 <c>MpModule</c>；三段式注册的第一段）。
/// </summary>
/// <remarks>
/// <para>
/// <b>成员即 <c>RegistryGroupName</c></b>：与 <c>[HttpClientApi(RegistryGroupName = …)]</c> 同名，
/// 源生成器据此产出 <c>Add{域}WebApiHttpClient()</c>。
/// </para>
/// <para>
/// <b>令牌签发不经本枚举</b>：小程序复用公众号线令牌底座（<c>AddMpApp</c> 自动注册令牌客户端），
/// 故本枚举只列业务域。
/// </para>
/// <para>
/// <b>与 Work 线 <c>Security</c> 组同名是既有事实、且无害</b>：生成的注册类为各程序集内的
/// <c>internal static class HttpClientApiExtensions</c>（命名空间分别为 <c>Mud.Wechat.Work</c> 与
/// <c>Mud.Wechat.MiniProgram</c>）⇒ 命名空间不同、可见性为内部，不构成公开面歧义，
/// 也不会让两个建造者产生同名方法（各自挂在各自的 <c>ServiceBuilder</c> 上）。
/// </para>
/// </remarks>
public enum MiniProgramModule
{
    /// <summary>
    /// 登录与用户（8 端点：<c>code2Session</c> + session_key 校验 / 重置 + 手机号 + 支付后 unionid
    /// + 插件用户 openpid + 检查加密信息 + 获取用户 encryptKey）。
    /// </summary>
    /// <remarks>
    /// 双接口同注册组：<c>IWxaAuthService</c> 7 端点带令牌 +
    /// <c>IWxaCode2SessionService</c> 1 端点免令牌（换会话走 appid + secret，不消费应用级令牌）。
    /// </remarks>
    Auth,

    /// <summary>
    /// 二维码 / 链接（9 端点：小程序码 3 + URL Link 2 + URL Scheme 2 + NFC Scheme 1 + ShortLink 1）。
    /// </summary>
    /// <remarks>
    /// <b>双通道</b>：URL Link / Scheme / NFC Scheme / ShortLink 六端点响应为 JSON（生成管线）；
    /// 小程序码三端点响应为<b>图片二进制流</b>（失败时才是 JSON）⇒ 走独立请求形态
    /// <c>IWxaCodeService</c>（Content-Type 分支判错），不进 JSON 反序列化管线。
    /// </remarks>
    QrCodeLink,

    /// <summary>
    /// 内容安全与风控（3 端点：文本同步审核 + 音视频异步审核 + 用户安全等级）。
    /// </summary>
    /// <remarks><c>/wxa/img_sec_check</c> 官方文档已下架（硬 404），刻意不实现。</remarks>
    Security,

    /// <summary>
    /// 数据分析（11 端点：访问趋势 3 + 访问留存 3 + 用户画像 1 + 访问分布 1 + 访问页面 1 + 数据概况 1 + 性能数据 1）。
    /// </summary>
    /// <remarks>应答信封不成一形（逐页核验的事实）：趋势 / 分布 / 页面为 <c>{ref_date, list[]}</c>、留存为两个数组、画像为两个对象 ⇒ 逐端点各自 DTO。</remarks>
    DataAnalysis,

    /// <summary>
    /// 订阅消息（4 端点：发送订阅消息 + 获取/激活服务卡片 + 更新服务卡片扩展信息）。
    /// </summary>
    /// <remarks>模板/类目/关键词等「订阅消息设置面」接口归公众号线 <c>NewTemplate</c> 域，本域只落发送与用户通知面。</remarks>
    SubscribeMessage,

    /// <summary>
    /// 动态消息（3 端点：创建 activity_id + 修改动态消息 + 修改聊天工具动态卡片消息）。
    /// </summary>
    DynamicMessage,

    /// <summary>
    /// 客服（11 端点：客服角色 2 + 客服子商户 4 + 微信客服绑定 3 + 客服输入状态 1 + 临时素材 1）。
    /// </summary>
    /// <remarks>
    /// <para>官方「小程序客服」目录跨三个子目录（客服消息 / 客服子商户 / 微信客服），
    /// 公众号线已占据客服账号 / 客服列表 / 发送客服消息 / 素材<b>上传</b>等端点，本域只补公众号线<b>未覆盖</b>的端点（单注册组）。</para>
    /// <para><b>2026-10-12 补漏订正</b>：公众号线<b>无</b> <c>/cgi-bin/media/get</c>（只有 <c>media/upload</c>），
    /// 且 <c>message/custom/business/typing</c> 与公众号线 <c>message/custom/typing</c> <b>路由不同</b>
    /// （官方按小程序 / 公众号二分），故两者均属本域缺口，本轮补入。
    /// 其中 <c>media/get</c> 成功返回<b>素材二进制流</b> ⇒ 走手工通道 <c>IWxaKfMediaService</c>。</para>
    /// </remarks>
    Kf,

    /// <summary>
    /// 硬件设备（9 端点：设备消息发送 + 票据 + 设备组 + License 管理）。
    /// </summary>
    /// <remarks>按官方「硬件设备」目录逐页收录；字段命名照抄官方原文拼写。</remarks>
    HardwareDevice,

    /// <summary>
    /// 运维中心（10 端点：域名配置 2 + 性能/来源/客户端版本 3 + 实时日志 1 + 错误日志 2 + 反馈列表 1 + 灰度发布 1 + 反馈图片 1）。
    /// </summary>
    /// <remarks>
    /// <b>反馈图片为独立请求形态</b>：<c>getfeedbackmedia</c> 成功时返回<b>图片二进制流</b>
    /// （失败时才是 JSON）⇒ 走 <c>IWxaFeedbackMediaService</c>（Content-Type 分支判错），
    /// 其余 9 端点走生成管线。
    /// </remarks>
    Operation,

    /// <summary>
    /// 插件管理（2 端点：插件申请管理 <c>/wxa/devplugin</c> + 插件管理 <c>/wxa/plugin</c>）。
    /// </summary>
    /// <remarks>两端点均 <c>action</c> 驱动（单路由多操作），DTO 按官方参数表建模。</remarks>
    Plugin,

    /// <summary>
    /// 付费管理（2 端点：资源包用量查询 + 最近平均用量查询）。
    /// </summary>
    Charge,

    /// <summary>
    /// 附近小程序（4 端点：添加地点 + 删除地点 + 查看地点列表 + 设置展示状态）。
    /// </summary>
    /// <remarks>添加后进入<b>审核</b>流程；<c>poi_id</c> 由添加结果返回并作为删除/展示状态的键。</remarks>
    NearbyPoi,

    /// <summary>
    /// 微信搜一搜（1 端点：搜一搜数据推送）。
    /// </summary>
    Search,

    /// <summary>
    /// 生物认证（1 端点：SOTER 生物认证秘钥签名验证）。
    /// </summary>
    Soter,

    /// <summary>
    /// 微信服务市场（2 端点：调用服务市场接口 + 异步获取处理数据）。
    /// </summary>
    ServiceMarket,

    /// <summary>
    /// 微信红包封面（1 端点：获取微信红包封面）。
    /// </summary>
    /// <remarks><c>ctoken</c> 为红包封面开放平台发放凭据，属敏感信息（禁止落日志）。</remarks>
    RedPacketCover,

    /// <summary>
    /// 学生身份（1 端点：快速获取学生身份）。
    /// </summary>
    Student,

    /// <summary>
    /// 微信人脸核身（2 端点：获取人脸核身会话唯一标识 + 查询真实验证结果）。
    /// </summary>
    /// <remarks><c>cert_info</c> 含证件姓名/号码等个人敏感信息（禁止落日志）；官方标注不支持第三方平台代调用。</remarks>
    FaceVerify,

    /// <summary>
    /// 用工关系（2 端点：推送用工消息 + 解绑用工关系）。
    /// </summary>
    LaborUse,

    /// <summary>
    /// 物流助手（68 端点：商家寄件 12 + 服务商/物流消息/退货/散单寄件/投诉处理/消息推送 28 + 同城配送 16 + 无忧退货 11 + 即时配送 15 + 对账文件 1）。
    /// </summary>
    /// <remarks>
    /// <para><b>商家寄件</b> 12 端点：下单 / 查询 / 批量查询 / 取消 / 轨迹 / 快递公司列表 / 快递账号 / 绑定账号 / 打印员 / 绑定打印员 / 面单余额 / 模拟轨迹（<c>express/business</c>）。</para>
    /// <para><b>服务商 / 物流消息 / 退货 / 散单寄件 / 投诉处理 / 消息推送</b> 28 端点（<c>express/delivery</c>）：面单联系人 / 面单预览 / 审核回传 / 轨迹同步 / 无忧退 3 / 物流消息 9 / 查询插件 4 / 退货组件 4 / 散单寄件 6（其中 <c>single_waybill/get_bill</c> 返回<b>对账文件文本</b> ⇒ 走手动通道 <c>IWxaExpressBillService</c>）/ 投诉处理 1 / 消息推送 2。</para>
    /// <para><b>同城配送</b> 16 端点（<c>express/intracity</c>）：开通 / 创建 / 查询 / 更新门店 + 运费充值 / 退款 / 流水 / 余额 + 询价 / 下单 / 查询 / 取消配送单 + 模拟回调（<c>mocknotify</c> 仅测试环境）+ 支持城市 / 查询扣费主体 / 设置扣费主体。</para>
    /// <para><b>无忧退货</b> 11 端点（<c>wxa/business/insurance_freight</c>）：开通 / 查询开通状态 / 发货投保 / 理赔 / 充值订单号 / 申请支付 / 充值订单列表 / 保费退款 / 理赔摘要 / 保单列表 / 告警余额。注意 <c>open</c> 与 <c>query_open</c> 官方为 <b>GET 且无请求体</b>、<c>refund</c> 为 <b>POST 且无请求体</b>。</para>
    /// <para><b>即时配送</b> 15 端点（<c>express/local</c>，小程序方 14 + 运力方 1）：该族返回体用 <c>resultcode</c>/<c>resultmsg</c> <b>而非</b> <c>errcode</c>/<c>errmsg</c> ⇒ 响应继承 <c>WxaExpressLocalResponse</c>。</para>
    /// </remarks>
    Express,

    /// <summary>
    /// 小程序会员服务（6 端点：<c>/wxa/vip/user/info/{add,update,get,delete}</c>、<c>/wxa/vip/user/list/get</c>、
    /// <c>/wxa/vip/shop/list/get</c>；小店→小程序会员同步的小程序侧开放面）。
    /// </summary>
    /// <remarks>
    /// <b>归属裁定（2026-10-12 逐页核对官方文档）</b>：6 端点虽出现在小店文档，但官方逐页标注
    /// 「小程序 API 请使用小程序身份，即小程序的 access_token 调用」（适用范围「小程序账号类型」，
    /// 第三方平台以 authorizer_access_token 代调用，权限集 185）⇒ 归本线（小店线设计方案 §4.1 结论 A，
    /// CH-V1 锁定小店线不得跨线声明）。
    /// </remarks>
    Vip,

    /// <summary>
    /// 交易保障（16 端点：基础 2 + 评价管理 9 + 投诉管理 5）。
    /// </summary>
    /// <remarks>
    /// <para><b>基础</b> 2 端点（<c>/wxaapi/wxamptrade/*</c>）：交易保障标状态（权限集 30、151）+ 交易体验分违规记录（权限集 151），均为 GET。</para>
    /// <para><b>评价管理</b> 9 端点（<c>/wxaapi/comment/*</c>，权限集 151）：评价列表 / 详情 / 评论回复列表 3 查（官方动词 GET）+ 评论增删 2 + 回复增删 2 + 确认和解 + 重置 API 客服 quota；官方对象分「评价 <c>commentId</c> → 评论 <c>replyId</c> → 回复 <c>commentReplyId</c>」三层，错误码为负数族（<c>-10403</c> 等）。</para>
    /// <para><b>投诉管理</b> 5 端点（<c>/wxaapi/minishop/*</c> 前缀为历史遗留命名，权限集 76）：查询投诉单详情（GET）+ 商家回应 / 补充凭证 / 提交退款凭证 / 申诉（POST）。路由拼写照官方原文：三个动作为 <c>bussi*</c>、申诉为 <c>busiAppeal</c>。<b>P3-R1 已据此收窄</b>：minishop 前缀由整族禁令改为「店铺 / 商品管理面零声明，官方仍在册的投诉处理 5 条在册」。</para>
    /// </remarks>
    TransactionGuarantee,

    /// <summary>
    /// 小程序直播（38 端点，六接口共用本注册组：直播间本体 11 + 小助手与主播副号 8 + 房间内商品 7 +
    /// 商品库管理 7 + 成员角色 3 + 长期订阅群发 2；路由族 <c>/wxaapi/broadcast/*</c> 与四支
    /// <c>/wxa/business/*</c>）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>P3-R1 第 2 项裁决的解除（2026-10-13 逐页取证）</b>：本域曾因「小程序直播插件已停止接入」被整族
    /// 零声明；取证推翻该前提 —— 38 个服务端 API 页面全部完整在册（各页声明权限集 52、服务器端调用、
    /// 云调用 <c>liveBroadcast.*</c>），现行目录树与旧 <c>miniprogram/dev/server/API/livebroadcast</c> 树
    /// 逐条相同；停止接入的是<b>直播插件</b>（前端组件），不是服务端 API。守卫
    /// <c>MiniProgramP3RulingsContractGuards</c> 的 broadcast 条款已随之收窄为清单式在册。
    /// </para>
    /// <para>
    /// <b>适用范围字段异常（诚实留档）</b>：38 页的「适用范围」段官方统一写「本接口暂未明确可调用账号类型」
    /// ——<b>无</b>「小程序」字样；本线归属判据取「令牌可用 <c>access_token</c> + 文档树位于
    /// <c>miniprogram/dev/server/API/</c>」两条。
    /// </para>
    /// </remarks>
    Live,

    /// <summary>
    /// 短剧媒资（31 端点，五接口共用本注册组：上传 6 + 媒资管理 4 + 剧目提审 7 + 用量与流量包 3 +
    /// 授权 11；路由族 <c>/wxa/sec/vod/*</c>，权限集 153）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入
    /// <c>access_token</c>，不新增令牌类型常量；第三方代调用形态（<c>authorizer_access_token</c>）由宿主
    /// 以小程序应用配置供给令牌（P3-R2）。
    /// </para>
    /// <para>
    /// <b>适用范围字段异常（诚实留档，与直播域同一处置）</b>：31 页的「适用范围」段官方统一写
    /// 「本接口暂未明确可调用账号类型」——<b>无</b>「小程序」字样；本线归属判据取「令牌可用
    /// <c>access_token</c> + 文档树位于 <c>miniprogram/dev/server/API/minidrama</c>」两条。
    /// </para>
    /// <para>
    /// <b>与视频号（Channels）线零交叠</b>：本域全部路由在 <c>/wxa/sec/vod/</c> 前缀下，与 <c>/channels/ec/*</c>
    /// 无任何交叠；「短剧」在两线是不同对象（本域是媒资剧目的提审与授权，视频号侧是内容合作带货达人），
    /// 守卫 DV1 断言跨线路由交叠为零。
    /// </para>
    /// </remarks>
    Vod,
}