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

    /// <summary>
    /// 虚拟支付（37 端点，六接口共用本注册组：代币操作 4 + 订单与账单 7 + 道具管理 5 + 会员订阅 4 +
    /// 广告金与提现 10 + 支付投诉 7；路由族 <c>/xpay/*</c>，权限集 157）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入
    /// <c>access_token</c>，不新增令牌类型常量；第三方代调用形态（<c>authorizer_access_token</c>）由宿主
    /// 以小程序应用配置供给令牌（P3-R2）。全域 37 端点<b>全部 POST、全部不支持云调用</b>，
    /// 除「商家解约」一页完全没有第三方调用段外，其余 36 页均声明代商家调用且权限集 id 为 157。
    /// </para>
    /// <para>
    /// <b>本域在令牌之外还有第二把 Query 凭据</b>：官方 28 端点强制 <c>pay_sig</c>（支付签名）、其中 3 端点
    /// 再叠加 <c>signature</c>（用户态签名）⇒ 签名值是<b>方法级显式参数</b>而非 <c>[Token]</c> 注入，
    /// 算法依赖 <c>session_key</c>、属宿主职责，SDK 不代签。<c>pay_sig</c> 不在组件脱敏静态词表内，
    /// 注册本模块时经 <c>XPaySensitiveQueryKeys.RegisterAll()</c> 登记为进程级强制掩码键（守卫 XP6）。
    /// </para>
    /// <para>
    /// <b>适用范围字段异常（诚实留档，与直播/短剧域同一处置）</b>：37 页的「适用范围」段官方统一写
    /// 「本接口暂未明确可调用账号类型，或在业务中根据调用传参自行确定是否可调用」——<b>无</b>「小程序」字样；
    /// 本线归属判据取「令牌可用 <c>access_token</c> + 文档树位于
    /// <c>miniprogram/dev/server/API/VirtualPayment</c>」两条。
    /// </para>
    /// <para>
    /// <b>官方本域无端点级频率数字</b>：37 页均未给「次/分钟」类数字，只有错误码 <c>268490015</c>「频率限制」
    /// 与订阅侧 <c>-15020</c>「操作过快，请稍候再试」证明确有限频 ⇒ 注释一律留档该空缺，
    /// <b>不得</b>凭第三方 SDK 注释或记忆补数。
    /// </para>
    /// <para>
    /// <b>与微信支付 APIv3（Pay 线）零交叠</b>：本域是<b>小程序虚拟支付</b>（米大师代币/道具，走
    /// <c>api.weixin.qq.com/xpay/*</c> + 小程序令牌），Pay 线是<b>微信支付商户 APIv3</b>（走
    /// <c>api.mch.weixin.qq.com</c> + 商户证书签名），两线的单号在订单模型里分列
    /// （<c>wxpay_order_id</c> 与 <c>channel_order_id</c>）⇒ 不得互相替代，守卫 XP4 断言跨线路由交叠为零。
    /// </para>
    /// </remarks>
    XPay,

    /// <summary>
    /// 发货信息管理与品牌申请（12 端点，四接口共用本注册组：发货录入与报备 4 + 订单与管理状态查询 4 +
    /// 设置与交易类型变更 2 + 品牌申请 2；路由族 <c>/wxa/sec/order/*</c> 10 条 + <c>/wxa/sec/famousbrand/*</c> 2 条）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>官方本域无端点级频率数字</b>：12 页均未给「次/分钟」类数字 ⇒ 注释一律留档该空缺，
    /// 不得凭第三方 SDK 注释或记忆补数；「<c>shipping_list</c> 多重性 [1,15]」「材料最多 10 个、视频最多 3 个」
    /// 「<c>page_size</c> 默认 100」均为<b>规模</b>限制，与频率分列。
    /// </para>
    /// <para>
    /// <b>第三方开放面在本域内三分</b>：10 端点支持 <c>authorizer_access_token</c>（权限集 142，其中
    /// <c>is_trade_managed</c> 与 <c>is_trade_management_confirmation_completed</c> 为 18、142），
    /// <c>get_order_list</c> 与 <c>set_msg_jump_path</c> 官方明写「本接口不支持第三方平台调用」。
    /// 云调用面同样不统一（7 支给出 <c>wxa.sec.order.*</c> 方法名、5 支明写不支持，且不支持的 5 支里
    /// 含第三方侧支持的 <c>is_trade_management_confirmation_completed</c> 与两支品牌申请）⇒ 逐端点看接口 XML 注释。
    /// </para>
    /// <para>
    /// <b>「一次机会」语义</b>：录入两支每笔支付单仅一次重新发货机会、确认收货提醒每单仅可调用一次、
    /// 交易类型变更一个账号只能申请一次 ⇒ 本域端点<b>一律非幂等</b>，不得放进自动重试管道。
    /// </para>
    /// <para>
    /// <b>与虚拟支付域零重叠</b>：本域订单定位用 <c>transaction_id</c> / <c>merchant_id</c> / <c>merchant_trade_no</c>
    /// （查询侧）与 <c>mchid</c> / <c>out_trade_no</c>（上传侧），与 <c>/xpay/query_order</c> 的单号面同名不同域，
    /// 状态取值表亦各一套 ⇒ 守卫 OS1 / XP4 分别锁定路由不交叉。
    /// </para>
    /// </remarks>
    OrderShipping,

    /// <summary>
    /// 小说作品管理（23 端点，五接口共用本注册组：作品本体 6 + 章节管理 8 + 授权管理 3 + 账号授权 3 + 读者运营 3；
    /// 路由族 <c>/wxa/book/*</c> 17 条 + <c>/wxa/business/novelreader/*</c> 3 条 + <c>/wxaapi/broadcast/*</c> 3 条）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>适用范围字段异常（诚实留档）</b>：23 页的「适用范围」段官方统一写「本接口暂未明确可调用账号类型」——
    /// <b>无</b>「小程序」字样；本线归属判据取「令牌可用 <c>access_token</c> + 文档树位于
    /// <c>miniprogram/dev/server/API/novelreadersetrecmdnovel</c>」两条。
    /// </para>
    /// <para>
    /// <b>权限集统一</b>：全域 23 端点均标注权限集 169，全部 POST，不支持云调用，支持第三方代调用。
    /// </para>
    /// <para>
    /// <b>官方本域无端点级频率数字</b>：23 页均未给「次/分钟」类数字 ⇒ 注释一律留档该空缺，
    /// 不得凭第三方 SDK 注释或记忆补数。
    /// </para>
    /// </remarks>
    Novel,

    /// <summary>
    /// 实名认证域（1 端点）：<c>/intp/realname/checkrealnameinfo</c>。
    /// </summary>
    /// <remarks>
    /// <para>业务方页面获得 code 之后，通过该接口进行实名信息的校验。</para>
    /// <para>不支持第三方平台代调用；官方未给出端点级频率数字。</para>
    /// </remarks>
    RealName,

    /// <summary>
    /// 小游戏域（20 端点：帧同步房间 4 + 用户数据存储 3 + 对局匹配 5 + 米大师支付 1 + 内容安全 2 + 米大师 V2 支付 5）。
    /// </summary>
    /// <remarks>
    /// <para><b>官方文档族</b>（minigame 树，2026-10-14 逐页核验）：帧同步房间 / 用户数据存储 / 对局匹配 / 米大师支付 / 内容安全 / 米大师 V2 支付。</para>
    /// <para><b>令牌面</b>：本域全部端点使用<b>小程序 access_token</b>（Query 注入，<c>MpTokenTypes.AccessToken</c>），
    /// 与小程序基础线同构；非第三方平台代调用形态（component_access_token 归开放平台线）。</para>
    /// <para><b>适用范围字段异常（诚实留档）</b>：20 页的「适用范围」段官方统一写「本接口暂未明确可调用账号类型」——
    /// <b>无</b>「小程序」字样；本线归属判据取「令牌可用 <c>access_token</c> + 文档树位于
    /// <c>minigame/dev/api-backend/</c>」两条。</para>
    /// </remarks>
    Game,

    /// <summary>
    /// 云开发（TCB）域（44 端点，四接口共用本注册组：环境与账号 8 + 数据库与云存储 16 + 云函数 9 + 其它能力 11；
    /// 路由族 <c>/tcb/*</c> 42 条 + <c>/wxa/getopendata</c> + <c>/wxa/getvoipsign</c>）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>官方文档树两处、路由一套</b>：小程序自研面位于
    /// <c>miniprogram/dev/server/API/cloudbase/{functions,database,storage,others}/*</c>，
    /// 公众平台通用面位于 <c>doc/oplatform/openApi/cloudbase-common/*</c> 与 <c>cloudbase-batch/*</c>；
    /// 同一 <c>/tcb/*</c> 路由在两处各有正文（2026-10-15 逐页核验）⇒ 本域端点不重复登记。
    /// </para>
    /// <para>
    /// <b>令牌面</b>：全域使用小程序 <c>access_token</c>（Query）。官方另给的 <c>authorizer_access_token</c>
    /// 与 <c>component_access_token</c> 只是代调用形态、不改令牌链；<b>仅</b>
    /// <c>component_access_token</c> 独占的 <c>/componenttcb/*</c> 面（50 端点）归开放平台线，
    /// 本域不得声明（守卫 CBZ 锁定两线不交叉）。
    /// </para>
    /// <para>
    /// <b>官方本域只给一处端点级频率数字</b>：<c>/tcb/getqcloudtoken</c> 为「10 次每小时」，
    /// 其余 43 端点未给数字 ⇒ 注释留档该空缺，不得凭第三方 SDK 注释或记忆补数。
    /// </para>
    /// <para>
    /// <b>被官方下线的两支不落地</b>：<c>/tcb/getappconfig</c> 与 <c>/tcb/uploadappconfig</c>
    /// （官方页面标题即标「即将下架」）⇒ 由零声明守卫登记，避免后续按旧清单回填。
    /// </para>
    /// </remarks>
    CloudBase,

    /// <summary>
    /// 自定义交易组件 V2·商店账号域（16 端点，七接口共用本注册组：账号与信息 4 + 审核 4 + 类目 1 +
    /// 图片上传 1 + 推客列表 1 + 入驻准入 4 + 场景校验 1；路由族 <c>/shop/account/*</c>、<c>/shop/audit/*</c>、
    /// <c>/shop/cat/get</c>、<c>/shop/img/upload</c>、<c>/shop/promoter/list</c>、<c>/shop/register/*</c>、<c>/shop/scene/check</c>）。
    /// </summary>
    /// <remarks>
    /// <para><b>令牌面</b>：全域小程序 <c>access_token</c>（Query），与小程序基础线同构；与小店线
    /// <c>/channels/ec/*</c> 的独立令牌天然隔离，本域不得声明含 <c>/channels/</c> 的路由。</para>
    /// <para><b>准入族两支共用同一官方页</b>：<c>/shop/register/apply</c> 与 <c>/shop/register/apply_scene</c>
    /// 官方同一文档（<c>enter/scene_apply.html</c>），差异只在请求体字段集 ⇒ 两个端点各自建模，不得合并。</para>
    /// <para><b>审核类端点「返回成功 ≠ 生效」</b>：品牌/类目审核与 <c>/shop/audit/result</c> 为异步终态口径，
    /// 逐端点注释留档；类目端点官方页面文件名即拼作 <c>get_children_cateogry.html</c>（官方原文拼写，照抄勿改）。</para>
    /// </remarks>
    ShopAccount,

    /// <summary>
    /// 自定义交易组件 V2·资金域（9 端点，三接口共用本注册组：余额与银行账户 3 + 订单流水 3 + 提现 3；
    /// 路由族 <c>/shop/funds/*</c>）。
    /// </summary>
    /// <remarks>
    /// <para><b>官方文档树两处、路由一套</b>：<c>/shop/funds/*</c> 的提现三支与扫码两支位于
    /// <c>ministore/wxafunds/API/funds/*</c> 分区，其余位于 <c>ministore/minishopopencomponent2/API/funds/*</c>
    /// 分区（逐页核验）⇒ 同一域承载，不重复登记。</para>
    /// <para><b>个人金融信息红线</b>：银行卡号、开户名、账户类型、提现金额、结算流水属个人金融信息，
    /// 一律不得写入日志、遥测或异常消息；金额单位口径逐端点钉住并守卫负锁。</para>
    /// </remarks>
    ShopFunds,

    /// <summary>
    /// 自定义交易组件 V2·分享员域（8 端点，路由族 <c>/shop/sharer/*</c>，全部 POST）。
    /// </summary>
    /// <remarks>
    /// <para><b>绑定/解绑为异步终态</b>：<c>bind</c> 与 <c>unbind</c> 返回成功不代表生效，终态由回调推送；
    /// 邀请上限 10000、频控「一天三次 / 十分钟一次」。</para>
    /// <para><b>官方表与示例键冲突（诚实留档）</b>：<c>/shop/sharer/set_share_mode</c> 官方参数表写作
    /// <c>sharer_mode</c>、请求示例写作 <c>share_mode</c> ⇒ 本仓取示例键，并由守卫负锁锁定该取舍；
    /// 切换为模式 2 后停止全族分享统计，30 天冷却（错误码 1310005）。</para>
    /// <para><b>同名不同型不得共用类型</b>：<c>live_start_time</c> 在订单支为 <c>string</c>、汇总支为 <c>long</c>。</para>
    /// </remarks>
    ShopSharer,

    /// <summary>
    /// 自定义交易组件 V2·商品域（9 端点，路由族 <c>/shop/spu/*</c>，全部 POST）。
    /// </summary>
    /// <remarks>
    /// <para><b>与 V1 商品域是两代契约</b>：本域为 <c>/shop/spu/*</c>（V2），
    /// <see cref="ProductCatalog"/> 与 <see cref="ProductSku"/> 为 <c>/product/*</c>（V1），
    /// 两代接口不得互相引用类型或路由。</para>
    /// <para><b>审核类端点返回成功 ≠ 生效</b>：<c>add</c> / <c>del</c> / <c>update</c> 走异步审核，
    /// 终态以 <c>get</c> 的 <c>status</c> 为准；<c>update_without_audit</c> 仅对已上架商品的部分字段免审，
    /// 适用范围受官方限制。删除与上下架均为非幂等状态迁移，不得自动重试。</para>
    /// </remarks>
    ShopSpu,

    /// <summary>
    /// 自定义交易组件 V2·优惠券域（11 端点，两接口共用本注册组：商家券 8 + 用户券 3；
    /// 路由族 <c>/shop/coupon/*</c>、<c>/shop/user_coupon/*</c>）。
    /// </summary>
    /// <remarks>
    /// <para><b>金额与门槛单位</b>：券面额与使用门槛一律为分；<c>discount_num</c> 为折扣千分比（官方口径照原文）。</para>
    /// <para><b>库存与状态机</b>：发券库存增减与券状态流转由官方异步结算，返回成功不代表终态；
    /// 商户侧须以 <c>get</c> / <c>list</c> 复核。</para>
    /// </remarks>
    ShopCoupon,

    /// <summary>
    /// 微信小程序「基础交易工具（原标准版交易组件 V1）」·商品域（22 端点，八接口共用本注册组：
    /// 商品本体与分类 5 + 配送与运费 4 + 线下与门店 3 + 售后与保障 3 + 服务市场与物流方式等 7；
    /// 路由族 <c>/product/*</c>，即官方 <c>minishopopencomponent/API</c> 分区）。
    /// </summary>
    /// <remarks>
    /// <para><b>与 V2（<c>/shop/*</c>）是两代契约</b>：本域为原「标准版交易组件」V1 面，V2 面在
    /// <see cref="ShopAccount"/> / <see cref="ShopFunds"/> / <see cref="ShopSharer"/> / <see cref="ShopCoupon"/> /
    /// <see cref="ShopOrder"/> 等注册组；两代接口<b>不得</b>互相引用类型或路由。</para>
    /// <para><b>令牌面</b>：全域小程序 <c>access_token</c>（Query），与 V2 同构。</para>
    /// </remarks>
    ProductCatalog,

    /// <summary>
    /// 微信小程序「基础交易工具（原标准版交易组件 V1）」·资金域（7 端点，路由族 <c>/product/funds/*</c>）。
    /// </summary>
    /// <remarks>
    /// <para><b>个人金融信息红线</b>：结算账户、提现与流水数据不得写入日志、遥测或异常消息；金额单位分。</para>
    /// </remarks>
    ProductFunds,

    /// <summary>
    /// 自定义交易组件 V2·订单域（20 端点，四接口共用本注册组：订单 8 + 发货 3 + 支付 6 + 微信支付进件 3；
    /// 路由族 <c>/shop/order/*</c>、<c>/shop/delivery/*</c>、<c>/shop/pay/*</c>、<c>/shop/wxpay/*</c>）。
    /// </summary>
    /// <remarks>
    /// <para><b>订单状态机</b>：10 待付款 / 11 收银台支付完成 / 20 待发货 / 21 部分发货 / 30 待收货 /
    /// 100 完成 / 181 超时未支付取消 / 200 售后取消 / 250 用户或商家取消（<c>WxaShopOrderStatuses</c>）。</para>
    /// <para><b>资金面</b>：<c>fund_type = 1</c>（二级商户单）才允许取支付参数（否则 1010047）；
    /// 支付与退款幂等口径逐端点注释留档。</para>
    /// </remarks>
    ShopOrder,

    /// <summary>
    /// 微信小程序「基础交易工具（原标准版交易组件 V1）」·SKU 域（17 端点，三接口共用本注册组：
    /// SKU 7 + SPU 8 + 库存 2；路由族 <c>/product/sku/*</c>、<c>/product/spu/*</c>、<c>/product/stock/*</c>）。
    /// </summary>
    /// <remarks>
    /// <para><b>金额单位</b>一律为分；<c>market_price</c> 必须严格大于 <c>sale_price</c>（9401055）。</para>
    /// <para><b>库存更新二选一</b>：全量覆盖幂等、增量更新不幂等；增量失败重试须先 <c>stock/get</c> 对账。</para>
    /// </remarks>
    ProductSku,

    /// <summary>
    /// 售后域（18 端点，三接口共用本注册组：V1 售后 4 + 交易纠纷 3 + V2 售后 11；
    /// 路由族 <c>/shop/aftersale/*</c>、<c>/shop/complaint/*</c>、<c>/shop/ecaftersale/*</c>）。
    /// </summary>
    /// <remarks>
    /// <para><b>V1 与 V2 两代并存</b>：<c>/shop/aftersale/*</c> 只认商家侧 <c>out_aftersale_id</c>、金额字段
    /// <c>refund</c>、状态 0~19；<c>/shop/ecaftersale/*</c> 有微信侧 <c>aftersale_id</c>、单商品
    /// <c>product_info</c>、金额字段 <c>orderamt</c>、状态 1~25 ⇒ 不得互抄字段与枚举。</para>
    /// <para><b>纠纷单只读 + 补材料</b>：<c>/shop/complaint/*</c> 无状态推进端点；官方未发布
    /// <c>state</c> / <c>type</c> / <c>judge_result</c> 枚举值，SDK 只建模 <c>int</c>。</para>
    /// <para><b>敏感面</b>：<c>openid</c> / <c>phone_number</c> / 退货地址 / 运单号 / 退款 ID
    /// 一律禁入日志、遥测与异常消息。</para>
    /// </remarks>
    ShopAfterSale,

    /// <summary>
    /// 微信小程序「基础交易工具（原标准版交易组件 V1）」·订单与地址域（14 端点，两接口共用本注册组：
    /// 地址 4 + 订单 10；路由族 <c>/product/address/*</c>、<c>/product/order/*</c>）。
    /// </summary>
    /// <remarks>
    /// <para><b>与 <see cref="ShopOrder"/>（V2 <c>/shop/order/*</c>）是两代契约</b>：本域订单状态值域
    /// 10/15/16/17/20/21/30/100/181/190/200/250、时间为 <c>yyyy-MM-dd HH:mm:ss</c>；
    /// V2 面状态值域 10/11/20/21/30/100/181/200/250、时间为毫秒字符串 ⇒ 不得互抄。</para>
    /// <para><b>售后三支的 <c>order_id</c> 是售后单号</b>（官方反直觉点）：<c>acceptapply</c> /
    /// <c>rejectrefund</c> / <c>upload_refund_certificate</c> 三页均只给示例、无参数表。</para>
    /// <para><b>两页 URL 互换（诚实留档）</b>：单条与批量售后单查询的官方页正文请求地址互为对方，
    /// 以路由语义为准。</para>
    /// <para><b>敏感面</b>：<c>openid</c> / 收件人姓名 / 手机号 / 详细地址一律禁入日志、
    /// 遥测与异常消息。</para>
    /// </remarks>
    ProductOrder,

    /// <summary>
    /// 微信小程序「基础交易工具（原标准版交易组件 V1）」·优惠券与限时抢购域（10 端点，两接口共用本注册组：
    /// 优惠券 7 + 限时抢购 3；路由族 <c>/product/coupon/*</c>、<c>/product/limiteddiscount/*</c>）。
    /// </summary>
    /// <remarks>
    /// <para><b>读写两侧不同型</b>：请求侧时间戳 / 数值枚举，响应侧字符串时间与
    /// <c>PROMOTE_TYPE_*</c> / <c>COUPON_VALID_TYPE_*</c> 符号串 ⇒ 两侧 DTO 分开。</para>
    /// <para><b>官方两页 <c>valid_type</c> 冲突</b>：create 页 1/2、update 页 0/1；
    /// update 页参数表与示例均未列 <c>coupon_id</c>，SDK 补齐定位键并留档。</para>
    /// <para><b>限时抢购归档在 <c>API/spu/</c> 但路由独立</b>；任务状态完整值域官方未发布，
    /// 只有 update_status 明示 1 结束 / 9 删除。</para>
    /// <para><b>敏感面</b>：<c>openid</c> 与用户券明细一律禁入日志、遥测与异常消息。</para>
    /// </remarks>
    ProductCoupon,

    /// <summary>
    /// 微信小程序「基础交易工具（原标准版交易组件 V1）」·装修与素材域（10 端点，三接口共用本注册组：
    /// 装修服务 6 + 图片上传 1（双请求形态）+ 管理二维码 3；路由族 <c>/product/decoration/*</c>、
    /// <c>/product/img/*</c>、<c>/product/qrcode/*</c>）。
    /// </summary>
    /// <remarks>
    /// <para><b>装修启停两段式</b>：<c>decoration_service</c> 返回成功仅表示已向管理员下发确认，
    /// 生效须轮询 <c>check_status</c>；压测每小商店每月 4 次配额。</para>
    /// <para><b>图片上传同路由双形态</b>：<c>upload_type=0</c> multipart、<c>=1</c> JSON；
    /// <c>resp_type</c> 决定返回类型与大小上限（2MB / 10MB）。</para>
    /// <para><b>二维码图片为 base64</b>；官方 check 页枚举表头重复、三页回包示例含中文弯引号，
    /// 均按参数表建模并留档。</para>
    /// </remarks>
    ProductMedia,

    /// <summary>
    /// 微信小游戏·米大师虚拟支付域（8 端点，单接口：正式环境 4 + 沙箱环境 4；
    /// 路由族 <c>/cgi-bin/midas/*</c>、<c>/cgi-bin/midas/sandbox/*</c>）。
    /// </summary>
    /// <remarks>
    /// <para><b>只承载 HTTPS 调用面</b>：官方同页另有云调用 <c>openapi.midas.*</c>，不在 SDK 范围。</para>
    /// <para><b><c>sig</c> 不代算</b>：HMAC-SHA256（全部参数 + uri + 米大师密钥）由调用方完成，
    /// SDK 只透传；密钥与签名原文一律禁入日志。</para>
    /// <para><b>幂等依赖 <c>bill_no</c></b>：扣币系统失败须用相同订单号重试；沙箱与正式环境数据隔离。</para>
    /// </remarks>
    Midas,

    /// <summary>
    /// 城市服务域（5 端点，两接口共用本注册组：适老化医疗 2 + 基础能力 3；
    /// 路由族 <c>/cityservice/*</c>）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>人脸核身三端点不落地</b>：<c>/cityservice/face/identify/{getimage,getinfo,getuseridkey}</c>
    /// 官方文档无现行页（旧页已下架），由零声明守卫登记；现役人脸核身面在
    /// <see cref="FaceVerify"/>（<c>getverifyid</c> / <c>queryverifyinfo</c>）。
    /// </para>
    /// <para><b>业务准入</b>：面向公立医院与卫健委等城市服务合作方，须完成行业资质报备。</para>
    /// <para>
    /// <b>敏感面</b>：<c>open_id</c>、医保授权码、密文姓名、就诊人信息与订单标识
    /// 一律禁入日志、遥测与异常消息。
    /// </para>
    /// </remarks>
    CityService,

    /// <summary>
    /// 行业能力域（5 端点，两接口共用本注册组：B2b 门店助手 3 + WMPF 硬件框架 2；
    /// 路由族 <c>/wxa/business/{batchcreateretail,getretailinfo,retailnotifybusiness}</c>、
    /// <c>/wxa/business/runtime/{adddevice,pushappmsg}</c>）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>令牌归属不统一</b>：B2b 族使用小程序 <c>access_token</c>；
    /// WMPF <c>adddevice</c> 必须使用移动应用（LauncherAPP）令牌，<c>pushappmsg</c> 两者皆可
    /// （官方原文）⇒ 由宿主应用配置保证，SDK 不校验归属。
    /// </para>
    /// <para>
    /// <b><c>/wxa/business/applyliveinfo</c> 不在本域</b>：该页官方明写仅支持「第三方平台」以
    /// <c>authorizer_access_token</c> 代调用、其他账号类型不可调用 ⇒ 属开放平台线
    /// <c>MiniProgramManage</c> 注册组（守卫 NC2 锁定本线零声明）。
    /// </para>
    /// <para>
    /// <b>官方文档树两处</b>：B2b 页在 <c>miniprogram/dev/server/API/B2b/</c>、
    /// WMPF 页在 <c>doc/oplatform/Miniprogram_Frame/</c>。
    /// </para>
    /// <para>
    /// <b>敏感面</b>：手机号、统一社会信用代码、法人 / 负责人姓名、<c>openid</c>、<c>push_token</c>
    /// 与推送消息体一律禁入日志、遥测与异常消息。
    /// </para>
    /// </remarks>
    BusinessCapability,
}