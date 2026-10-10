// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Text.Json;

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 查询到的记录（官方 Record；查询记录响应的 <c>records</c> 元素）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class SmartSheetRecord
{
    /// <summary>获取或设置记录 ID（官方 <c>record_id</c>）。</summary>
    [JsonPropertyName("record_id")]
    public string? RecordId { get; set; }

    /// <summary>获取或设置记录的创建时间（官方 <c>create_time</c>）。</summary>
    [JsonPropertyName("create_time")]
    public string? CreateTime { get; set; }

    /// <summary>获取或设置记录的更新时间（官方 <c>update_time</c>）。</summary>
    [JsonPropertyName("update_time")]
    public string? UpdateTime { get; set; }

    /// <summary>
    /// 获取或设置记录具体内容（官方 <c>values</c>）。
    /// key 为字段标题或字段 ID（由请求的 <c>key_type</c> 决定），value 须按字段类型构造：文本 <c>FIELD_TYPE_TEXT</c> 为对象数组（<c>type</c> / <c>text</c> / <c>link</c>）、数字 <c>FIELD_TYPE_NUMBER</c> 为数字、复选框 <c>FIELD_TYPE_CHECKBOX</c> 为布尔、日期 <c>FIELD_TYPE_DATE_TIME</c> 为 Unix 毫秒时间戳字符串、图片 <c>FIELD_TYPE_IMAGE</c> 为对象数组、文件 <c>FIELD_TYPE_ATTACHMENT</c> 为对象数组、成员 <c>FIELD_TYPE_USER</c> 为对象数组、链接 <c>FIELD_TYPE_URL</c> 为对象数组、多选 <c>FIELD_TYPE_SELECT</c> 为选项对象数组、进度 <c>FIELD_TYPE_PROGRESS</c> 为数字、电话 <c>FIELD_TYPE_PHONE_NUMBER</c> 为字符串、邮箱 <c>FIELD_TYPE_EMAIL</c> 为字符串、单选 <c>FIELD_TYPE_SINGLE_SELECT</c> 为选项对象数组、地理位置 <c>FIELD_TYPE_LOCATION</c> 为对象数组（长度不大于 1）、关联 <c>FIELD_TYPE_REFERENCE</c> 为记录 ID 字符串数组、货币 <c>FIELD_TYPE_CURRENCY</c> 为数字、自动编号 <c>FIELD_TYPE_AUTONUMBER</c> 为对象数组（<c>seq</c> / <c>text</c>）、百分数 <c>FIELD_TYPE_PERCENTAGE</c> 为数字、条码 <c>FIELD_TYPE_BARCODE</c> 为字符串。
    /// 官方说明：链接类型的数组为预留能力，目前只支持展示一个链接，建议只传入一个链接。
    /// 因官方 value 为异构 JSON（同一 map 内不同 key 的值类型不同），本模型以 <see cref="JsonElement"/> 承载，调用方可用 <c>JsonSerializer.SerializeToElement</c> 或 <c>JsonDocument.Parse(...).RootElement.Clone()</c> 构造。
    /// </summary>
    [JsonPropertyName("values")]
    public Dictionary<string, JsonElement>? Values { get; set; }

    /// <summary>获取或设置创建者名字（官方 <c>creator_name</c>）。</summary>
    [JsonPropertyName("creator_name")]
    public string? CreatorName { get; set; }

    /// <summary>获取或设置最后编辑者名字（官方 <c>updater_name</c>）。</summary>
    [JsonPropertyName("updater_name")]
    public string? UpdaterName { get; set; }
}
