// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.DataModels.Common;

/// <summary>
/// 微信支付 APIv3 统一响应基底（承载官方错误信封 <c>code</c> / <c>message</c>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>为何所有支付响应都继承本类</b>：APIv3 的<b>成功</b>响应体<b>不含</b> <c>code</c> 字段，
/// <b>失败</b>响应体恒为 <c>{"code":"…","message":"…"}</c> 且 HTTP 状态为 4xx/5xx。
/// 支付接口带 <c>[AllowAnyStatusCode]</c>（否则组件会在 4xx 直接抛 <c>ApiException</c>、
/// 把官方业务码丢掉），错误体因此<b>原样反序列化</b>进本基类的 <see cref="Code"/> / <see cref="Message"/>，
/// 调用方据此经 <c>WechatPayException.ThrowIfFailed</c> 判错。
/// </para>
/// <para>
/// <b>与公众号 <c>MpResponse</c> 的差异</b>：公众号是「HTTP 200 + 整数 <c>errcode</c>」，
/// 成功也可能带 <c>errcode=0</c>；支付是「4xx + 字符串 <c>code</c>」，成功<b>整字段缺省</b>。
/// 故 <see cref="IsSuccess"/> 的判定是「<see cref="Code"/> 为空」，而非整数比较。
/// </para>
/// <para>
/// <b>int 槽位（<see cref="ErrorCode"/>）</b>：公用层判错契约要求 <see cref="int"/>，而 APIv3 无整数码，
/// 故实现为「成功 0 / 失败 <c>-1</c>」哨兵；权威码见 <see cref="Code"/>
/// （理由见 <c>WechatPayException</c> remarks）。
/// </para>
/// <para>
/// <b>基底 DTO 为何留在产品线而不下沉公用层</b>：各产品线错误信封形态不一致，下沉的只能是
/// 「判错面」这一非序列化契约（<c>IWechatApiResponse</c>），基底 DTO 归各产品线 —— 与公众号形态一致。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Common")]
public class WechatPayResponse : IWechatApiResponse
{
    /// <summary>
    /// 官方 APIv3 错误码（<c>code</c>，字符串）。<b>成功响应整字段缺省</b>（非空串）。
    /// </summary>
    [JsonPropertyName("code")]
    public virtual string? Code { get; set; }

    /// <summary>官方 APIv3 错误描述（<c>message</c>，成功响应缺省）。</summary>
    [JsonPropertyName("message")]
    public virtual string? Message { get; set; }

    /// <inheritdoc />
    /// <remarks>APIv3 无整数错误码：成功恒 <c>0</c>、失败恒 <c>-1</c>。分类请用 <see cref="Code"/>。</remarks>
    [JsonIgnore]
    public int ErrorCode => IsSuccess ? 0 : WechatPayFailureSentinel;

    /// <inheritdoc />
    [JsonIgnore]
    public string? ErrorMessage => Message;

    /// <inheritdoc />
    /// <remarks><see cref="Code"/> 为空即视为成功（成功响应体不含 <c>code</c>）。</remarks>
    [JsonIgnore]
    public bool IsSuccess => string.IsNullOrEmpty(Code);

    /// <summary>失败时填充 <see cref="ErrorCode"/> 的哨兵值（与 <c>WechatPayException.NumericFailureSentinel</c> 同值）。</summary>
    private const int WechatPayFailureSentinel = -1;
}
