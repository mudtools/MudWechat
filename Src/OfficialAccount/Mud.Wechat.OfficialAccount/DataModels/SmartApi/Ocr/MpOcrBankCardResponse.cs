// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.SmartApi;

/// <summary>
/// 银行卡识别（<c>POST /cv/ocr/bankcard</c>）响应。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档矛盾（照录，SDK 以超集承载不漏数据）</b>：返回参数表报字段名为 <c>number</c>（银行卡号），
/// 但官方 CURL 与云函数两处返回示例实际输出的键为 <c>id</c>。SDK 同时保留
/// <see cref="Number"/> 与 <see cref="Id"/> 两个属性（按报文实际形态取值），不做归一化。
/// </para>
/// <para>
/// 官方错误码：<c>-1</c> / <c>40001</c> / <c>101000</c> / <c>101001</c> / <c>101003</c>。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "SmartApi")]
public class MpOcrBankCardResponse : MpResponse
{
    /// <summary>获取或设置银行卡号（官方字段表字段名 <c>number</c>；返回示例未使用该键）。</summary>
    [JsonPropertyName("number")]
    public string? Number { get; set; }

    /// <summary>获取或设置银行卡号（官方返回示例实际使用的键 <c>id</c>；字段表未列）。</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }
}
