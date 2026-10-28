// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Ams;

/// <summary>
/// 微信开放平台「流量主代运营（AMS）」域响应基底。
/// </summary>
/// <remarks>
/// <para><b>为何独立于 <see cref="OpenPlatformResponse"/>（判错契约同、JSON 形状异）</b>：
/// 本域全部端点（2026-10-17 逐页核验 <c>ams/*</c> 约 22 页）的应答信封是 <c>{ret, err_msg}</c>，
/// 而<b>非</b>本线其它域的 <c>{errcode, errmsg}</c>。若强行套 <see cref="OpenPlatformResponse"/>，
/// 则其 <c>errcode</c> 永不绑定、<c>IsSuccess</c>（判 <c>errcode==0</c>）恒真，
/// <c>ret != 0</c> 的业务失败会被静默吞掉。故本域另立基底，判错面仍走公用层
/// <see cref="Mud.Wechat.Abstractions.Contracts.IWechatApiResponse"/>（<c>ErrorCode =&gt; Ret</c>）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Ams")]
public class OpenPlatformAmsResponse : Mud.Wechat.Abstractions.Contracts.IWechatApiResponse
{
    /// <summary>获取或设置本域业务返回码（官方字段名 <c>ret</c>，number；0 表示成功）。</summary>
    [JsonPropertyName("ret")]
    public virtual int Ret { get; set; }

    /// <summary>获取或设置本域业务错误信息（官方字段名 <c>err_msg</c>）。</summary>
    [JsonPropertyName("err_msg")]
    public virtual string? ErrorMessage { get; set; }

    /// <summary>获取本域业务错误码（等价 <see cref="Ret"/>，为公用层判错契约提供视图）。</summary>
    [JsonIgnore]
    public int ErrorCode => Ret;

    /// <summary>获取一个值，该值表示本次调用是否成功（<see cref="Ret"/> == 0）。</summary>
    [JsonIgnore]
    public bool IsSuccess => Ret == 0;
}
