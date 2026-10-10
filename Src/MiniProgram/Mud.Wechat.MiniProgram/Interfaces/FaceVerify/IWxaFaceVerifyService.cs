// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「微信人脸核身」域 SDK（2 端点：获取人脸核身会话唯一标识 + 查询真实验证结果）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>（小程序服务端 API → 微信人脸核身，2026-10-10 依据官方清单核验）：
/// <c>face/api_getverifyid.html</c>、<c>face/api_queryverifyinfo.html</c>。
/// </para>
/// <para>
/// <b>业务形态（官方原文）</b>：业务方后台根据「用户实名信息（姓名 + 证件号）」调用
/// <see cref="GetVerifyIdAsync"/> 获取人脸核身会话唯一标识 <c>verify_id</c>，交给小程序前端拉起
/// 人脸核身；核身完成后经 <see cref="QueryVerifyInfoAsync"/> 查询<b>真实</b>验证结果。
/// </para>
/// <para>
/// <b>令牌与鉴权</b>：全部走应用级 <c>access_token</c>（Query）。两端点官方均为 <b>POST</b>；
/// 官方标注本域<b>不支持第三方平台代调用</b>。
/// </para>
/// <para>
/// <b>敏感信息警示（MP-X7）</b>：<c>cert_info</c> 含<b>证件姓名与证件号码</b>等个人敏感信息，
/// 禁止写入日志、遥测或异常消息；调用完成即应丢弃。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "FaceVerify", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaFaceVerifyService
{
    /// <summary>
    /// 获取用户人脸核身会话唯一标识。官方文档：<c>face/api_getverifyid.html</c>。
    /// </summary>
    /// <param name="request">获取请求（<c>out_seq_no</c> / <c>openid</c> / <c>cert_info</c> 必填），见 <see cref="DataModels.FaceVerify.WxaFaceGetVerifyIdRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>会话唯一标识（<c>verify_id</c>）与其有效期（<c>expires_in</c>），见 <see cref="DataModels.FaceVerify.WxaFaceGetVerifyIdResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/cityservice/face/identify/getverifyid</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para>
    /// <b>流程（官方原文）</b>：后台凭用户实名信息（姓名 + 证件号）调用本接口取得 <c>verify_id</c>，
    /// 再交给小程序前端调用人脸核身组件完成核身；<c>verify_id</c> 带有效期（<c>expires_in</c>，秒）。
    /// </para>
    /// <para><b>敏感信息警示</b>：<c>cert_info</c>（证件姓名/号码）禁止写入日志、遥测或异常消息。</para>
    /// </remarks>
    [Post("/cityservice/face/identify/getverifyid")]
    Task<DataModels.FaceVerify.WxaFaceGetVerifyIdResponse> GetVerifyIdAsync(
        [Body] DataModels.FaceVerify.WxaFaceGetVerifyIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询用户人脸核身真实验证结果。官方文档：<c>face/api_queryverifyinfo.html</c>。
    /// </summary>
    /// <param name="request">查询请求（<c>out_seq_no</c> / <c>verify_id</c> / <c>openid</c> / <c>cert_hash</c> 必填），见 <see cref="DataModels.FaceVerify.WxaFaceQueryVerifyInfoRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>验证结果码（<c>verify_ret</c>），见 <see cref="DataModels.FaceVerify.WxaFaceQueryVerifyInfoResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> <c>/cityservice/face/identify/queryverifyinfo</c> ＋ 请求体 JSON；Query 携带 <c>access_token</c>。</para>
    /// <para><b>核身通过判据（官方原文）</b>：须<b>同时</b>满足 <c>errcode == 0</c> 与 <c>verify_ret == 10000</c>。</para>
    /// <para><b>证件摘要</b>：<c>cert_hash</c> 由 <see cref="DataModels.FaceVerify.WxaFaceQueryVerifyInfoRequest.SetCertHash"/> 计算（内存内哈希，不记录明文）。</para>
    /// </remarks>
    [Post("/cityservice/face/identify/queryverifyinfo")]
    Task<DataModels.FaceVerify.WxaFaceQueryVerifyInfoResponse> QueryVerifyInfoAsync(
        [Body] DataModels.FaceVerify.WxaFaceQueryVerifyInfoRequest request,
        CancellationToken cancellationToken = default);
}