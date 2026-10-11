// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.DataModels.Kf;

namespace Mud.Wechat.Channels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）「商家客服」域 SDK（2 端点：多媒体资源上传 / 客服消息发送）。
/// </summary>
/// <remarks>
/// <para>
/// <b>与「企微微信客服」概念严格区分</b>：本域是微信小店的商家客服（<c>/channels/ec/commkf/*</c>，
/// 小店 <c>access_token</c>），<b>不是</b>企微线的微信客服（Work 线 <c>Interfaces/KF/</c>，
/// 企微 <c>Wechat.AccessToken</c>）。两条线无任何路由交叠，命名空间亦分线隔离。
/// </para>
/// <para>
/// <b>客服消息时序契约</b>：用户发消息后，客服仅可在 48 小时内回复且最多回复 5 条（超出分别报
/// 10190009 / 10190008）；用户结束会话后不可继续发送（10190013）。
/// </para>
/// <para>
/// <b>媒体上传形态</b>：官方多媒体资源上传为 <c>multipart/form-data</c>（消息类型 / openid / 文件
/// 均为表单字段），SDK 按库内既有的声明式上传通路建模（<see cref="IFormContent"/>，文件字段
/// 与 <c>msg_type</c> / <c>open_id</c> 表单字段均由调用方构建）。
/// </para>
/// <para>
/// <b>MUD005 已知接受风险</b>：微信小店官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Kf", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsKfService
{
    /// <summary>
    /// 上传多媒体资源（<c>cosupload</c>，商家客服会话用图 / 视 / 文件，返回 cos_url）。
    /// </summary>
    /// <param name="formData">multipart 表单内容（由调用方构建 <see cref="IFormContent"/> 实现）——
    /// 官方表单字段：<c>msg_type</c>（文件类型，见 <see cref="ChannelsKfMediaTypes"/>，必填）、
    /// <c>open_id</c>（用户 openid，必填）、<c>file</c>（待上传的视频 / 图片 / 文件，必填）。
    /// 须标 <c>[MultipartForm]</c> —— 走 <see cref="IFormContent.ToHttpContentAsync"/> 通路（与
    /// Work 线媒体上传同形态）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>多媒体资源地址（<c>cos_url</c>，形如 channels.weixin.qq.com/shop/commkf/downloadmedia?encrypted_param=…，供发送消息接口引用）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/kf/api_cosupload"/></para>
    /// <para>
    /// 官方业务限制：①官方请求体为 <c>multipart/form-data</c>（msg_type / open_id / file 均为表单字段），
    /// 不支持云调用，不支持第三方平台调用；②图片 / 视频 / 文件大小均<b>不超过 30MB</b>；
    /// ③上传的媒体文件在微信服务器上<b>保存 14 天</b>，过期后自动删除。
    /// </para>
    /// <para>官方错误码：<c>10080000</c>（注销公示期）/ <c>10190001</c>（系统错误）。</para>
    /// <para>适用范围：仅「微信小店（需申请）」账号类型可调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/commkf/cosupload")]
    Task<ChannelsKfCosUploadResponse> CosUploadAsync(
        [MultipartForm] IFormContent formData,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送消息（<c>sendmsg</c>，商家客服向指定用户发送客服消息）。
    /// </summary>
    /// <param name="request">发送请求（<c>msg_type</c> 必填且决定哪个内容对象必填，见 <see cref="ChannelsKfMsgTypes"/>；
    /// 媒体类内容的 <c>cos_url</c> 须经上传多媒体资源接口获取）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>消息的单调自增 id（<c>msg_id</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/kf/api_sendmsg"/></para>
    /// <para>
    /// 官方业务限制：①用户发消息后，客服只可在 <b>48 小时内</b>回复用户且<b>最多可回复 5 条消息</b>
    /// （超出分别报 10190009 / 10190008）；②用户已结束会话后不能继续发送（10190013）；
    /// ③消息发送经过风控、数据反垃圾，须注意内容安全（10190010）；④涉及用户联系方式、地址等聊天内容
    /// 会按《个人信息保护法》自动加密处理；⑤<paramref name="request"/> 中 <c>request_id</c>
    /// 填写时以该任务 id 去重。
    /// </para>
    /// <para>官方错误码：<c>10080000</c>（注销公示期）/ <c>10190001</c>（系统错误）/ <c>10190002</c>（须 POST）/
    /// <c>10190003</c>（参数错误）/ <c>10190006</c>（请求体转 JSON 失败）/ <c>10190008</c>（消息条数超限）/
    /// <c>10190009</c>（会话超过 48h）/ <c>10190010</c>（UCG 消息命中风控）/ <c>10190013</c>（用户已结束会话）。</para>
    /// <para>适用范围：仅「微信小店（需申请）」账号类型可调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/commkf/sendmsg")]
    Task<ChannelsKfSendMsgResponse> SendMessageAsync(
        [Body] ChannelsKfSendMsgRequest request,
        CancellationToken cancellationToken = default);
}
