// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Wedoc;

/// <summary>
/// 收集表问题项（官方 <c>form_question.items</c> 元素）。
/// </summary>
/// <remarks>
/// <para>
/// 官方业务限制：问题 id（question_id）从 1 开始，家校范围收集表从 2 开始；
/// <c>reply_type</c> 题型取值：<c>1</c> 文本、<c>2</c> 单选、<c>3</c> 多选、<c>5</c> 位置、<c>9</c> 图片、<c>10</c> 文件、
/// <c>11</c> 日期、<c>14</c> 时间、<c>15</c> 下拉列表、<c>16</c> 体温、<c>17</c> 签名、<c>18</c> 部门、<c>19</c> 成员、<c>22</c> 时长；
/// <c>option_item</c> 为单选/多选/下拉列表题的选项列表。
/// </para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Wedoc")]
public class WedocFormItem
{
    /// <summary>获取或设置问题 id（官方 <c>question_id</c>，必填），从 1 开始；家校范围收集表从 2 开始。</summary>
    [JsonPropertyName("question_id")]
    public uint? QuestionId { get; set; }

    /// <summary>获取或设置问题描述（官方 <c>title</c>，必填）。</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>获取或设置问题序号（官方 <c>pos</c>，必填），从 1 开始。</summary>
    [JsonPropertyName("pos")]
    public uint? Pos { get; set; }

    /// <summary>
    /// 获取或设置问题状态（官方 <c>status</c>，必填）。
    /// 官方取值：<c>1</c> 正常、<c>2</c> 被删除。
    /// </summary>
    [JsonPropertyName("status")]
    public uint? Status { get; set; }

    /// <summary>
    /// 获取或设置题型（官方 <c>reply_type</c>，必填）。
    /// 官方取值：<c>1</c> 文本、<c>2</c> 单选、<c>3</c> 多选、<c>5</c> 位置、<c>9</c> 图片、<c>10</c> 文件、
    /// <c>11</c> 日期、<c>14</c> 时间、<c>15</c> 下拉列表、<c>16</c> 体温、<c>17</c> 签名、<c>18</c> 部门、<c>19</c> 成员、<c>22</c> 时长。
    /// </summary>
    [JsonPropertyName("reply_type")]
    public uint? ReplyType { get; set; }

    /// <summary>获取或设置是否必答（官方 <c>must_reply</c>，必填）。</summary>
    [JsonPropertyName("must_reply")]
    public bool? MustReply { get; set; }

    /// <summary>获取或设置问题备注（官方 <c>note</c>）。</summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }

    /// <summary>获取或设置问题填写提示（官方 <c>placeholder</c>），响应中返回。</summary>
    [JsonPropertyName("placeholder")]
    public string? Placeholder { get; set; }

    /// <summary>获取或设置单选/多选/下拉列表题的选项列表（官方 <c>option_item</c>）。</summary>
    [JsonPropertyName("option_item")]
    public List<WedocFormOptionItem>? OptionItem { get; set; }

    /// <summary>获取或设置各题型的额外设置（官方 <c>question_extend_setting</c>），按 <c>reply_type</c> 取对应设置对象。</summary>
    [JsonPropertyName("question_extend_setting")]
    public WedocFormExtendSetting? QuestionExtendSetting { get; set; }
}
