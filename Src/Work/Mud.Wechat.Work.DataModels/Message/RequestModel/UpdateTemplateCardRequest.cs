// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Message;

/// <summary>
/// 更新模版卡片消息请求体（<c>/cgi-bin/message/update_template_card</c>，官方文档 94888 自建 / 94945 第三方 / 96459 代开发）。
/// <para>仅原卡片为按钮交互型、投票选择型、多项选择型，以及填写了 action_menu 字段的文本通知型、图文展示型可以更新；
/// <c>response_code</c> 一个只能调用一次本接口，且只能在 72 小时内调用。</para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Message")]
public class UpdateTemplateCardRequest
{
    /// <summary>
    /// 获取或设置企业的成员 ID 列表（最多支持 1000 个）。
    /// </summary>
    [JsonPropertyName("userids")]
    public List<string>? UserIds { get; set; }

    /// <summary>
    /// 获取或设置企业的部门 ID 列表（最多支持 100 个）。
    /// </summary>
    [JsonPropertyName("partyids")]
    public List<int>? PartyIds { get; set; }

    /// <summary>
    /// 获取或设置企业的标签 ID 列表（最多支持 100 个）。
    /// </summary>
    [JsonPropertyName("tagids")]
    public List<int>? TagIds { get; set; }

    /// <summary>
    /// 获取或设置是否更新整个任务接收人员（1 表示生效）。
    /// </summary>
    [JsonPropertyName("atall")]
    public int? AtAll { get; set; }

    /// <summary>
    /// 获取或设置应用的 agentid（官方必填）。
    /// </summary>
    [JsonPropertyName("agentid")]
    public int? AgentId { get; set; }

    /// <summary>
    /// 获取或设置更新卡片所需要消费的 code（官方必填），可通过发消息接口和回调接口返回值获取，
    /// 一个 code 只能调用一次该接口，且只能在 72 小时内调用。
    /// </summary>
    [JsonPropertyName("response_code")]
    public string? ResponseCode { get; set; }

    /// <summary>
    /// 获取或设置是否开启 id 转译：0 - 否（默认），1 - 是。
    /// </summary>
    [JsonPropertyName("enable_id_trans")]
    public int? EnableIdTrans { get; set; }

    /// <summary>
    /// 获取或设置模式一：更新按钮为不可点击状态
    /// （仅原卡片为按钮交互型、投票选择型、多项选择型可以更新按钮）。
    /// </summary>
    [JsonPropertyName("button")]
    public UpdateTemplateCardButton? Button { get; set; }

    /// <summary>
    /// 获取或设置模式二：更新为新的卡片（可回调的卡片可以更新成任何一种模板卡片）。
    /// </summary>
    [JsonPropertyName("template_card")]
    public TemplateCardBody? TemplateCard { get; set; }
}
