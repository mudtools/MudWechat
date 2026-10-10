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
    /// 客服（9 端点：客服角色 2 + 客服子商户 4 + 微信客服绑定 3）。
    /// </summary>
    /// <remarks>官方「小程序客服」目录跨三个子目录（客服消息 / 客服子商户 / 微信客服），
    /// 公众号线已占据客服账号/消息/素材等端点，本域只补公众号线<b>未覆盖</b>的 9 端点（单注册组）。</remarks>
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
}