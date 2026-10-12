// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Express;

/// <summary>面单数据项（<c>waybill_data</c> 数组元素，键值对）。</summary>
/// <remarks>键值集合由快递公司定义（各运力不一），SDK 不做本地校验，原样透传。</remarks>
[HttpJsonSerializable(SerializerClassName = "Express")]
public class WxaExpressWaybillDataItem
{
    /// <summary>面单数据键（<c>key</c>）。</summary>
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    /// <summary>面单数据值（<c>value</c>）。</summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}
