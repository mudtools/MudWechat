// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.WeData;

/// <summary>
/// 用户 PC 端登录应答（<c>POST /wedata/wedata_login</c>）。
/// </summary>
/// <remarks>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/we-analysis/wedata_login.html"/></para>
/// <para><b>判错形状不同于本线其它端点</b>：本应答<b>没有</b>顶层 <c>errcode/errmsg</c>，
/// 业务码在 <c>base_resp</c> 内（<c>ret</c>/<c>err_msg</c>）⇒ 直接实现
/// <see cref="Mud.Wechat.Abstractions.Contracts.IWechatApiResponse"/>，以 <c>base_resp.ret</c> 判成败。
/// 登录链接 <c>redirect_url</c> 为一次性凭据，<b>不得</b>落日志或遥测。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "WeData")]
public class OpenPlatformWeDataLoginResponse : Mud.Wechat.Abstractions.Contracts.IWechatApiResponse
{
    /// <summary>基础应答信息（官方 <c>base_resp</c>）。</summary>
    [JsonPropertyName("base_resp")]
    public OpenPlatformWeDataBaseResponse? BaseResp { get; set; }

    /// <summary>We 分析登录链接（官方 <c>redirect_url</c>）。</summary>
    [JsonPropertyName("redirect_url")]
    public string? RedirectUrl { get; set; }

    /// <summary>登录链接过期时间戳（官方 <c>expire_at</c>，Unix 秒）。</summary>
    [JsonPropertyName("expire_at")]
    public long? ExpireAt { get; set; }

    /// <summary>获取业务错误码（视图 <see cref="OpenPlatformWeDataBaseResponse.Ret"/>）。</summary>
    [JsonIgnore]
    public int ErrorCode => BaseResp?.Ret ?? 0;

    /// <summary>获取业务错误消息（视图 <see cref="OpenPlatformWeDataBaseResponse.ErrorMessage"/>）。</summary>
    [JsonIgnore]
    public string? ErrorMessage => BaseResp?.ErrorMessage;

    /// <summary>获取一个值，该值表示本次调用是否成功（<see cref="OpenPlatformWeDataBaseResponse.Ret"/> == 0）。</summary>
    [JsonIgnore]
    public bool IsSuccess => ErrorCode == 0;
}
