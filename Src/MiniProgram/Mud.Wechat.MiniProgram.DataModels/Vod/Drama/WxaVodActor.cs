// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vod;

/// <summary>
/// 演员信息（官方 <c>Body.actor_list.actor(Array)</c>，四字段全必填）。
/// </summary>
/// <remarks>
/// <para><b>字符集与长度是官方硬约束</b>（照录原文）：姓名 30 字符、支持中英文和「·」；饰演角色 30 字符；简介 100 字符且需涵盖生日/星座/籍贯/身高/毕业院校/作品/获奖等要素中至少 3 项（或任一要素不少于 20 字）。SDK 不本地校验。</para>
/// <para>官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/minidrama/auditdrama/api_auditdrama.html"/></para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Vod")]
public class WxaVodActor
{
    /// <summary>name（官方类型 <c>string</c>，<b>必填</b>）：演员姓名，格式要求：可填写30个字符，支持输入中文、英文和·</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>photo_material_id（官方类型 <c>string</c>，<b>必填</b>）：演员照片临时material_id</summary>
    [JsonPropertyName("photo_material_id")]
    public string? PhotoMaterialId { get; set; }

    /// <summary>role（官方类型 <c>string</c>，<b>必填</b>）：饰演角色，格式要求：可填写30个字符，支持输入中文、英文及，。；”“？、—《》！（）-·</summary>
    [JsonPropertyName("role")]
    public string? Role { get; set; }

    /// <summary>profile（官方类型 <c>string</c>，<b>必填</b>）：演员简介，填写内容可参考：生日、星座、籍贯、身高、毕业院校、历史/代表作品、获奖信息、成就等(以上要素需至少满足3项或涵盖任一要素且不少于20字）。 格式要求：可填写100个字符，支持输入中文、英文、阿拉伯数字及，。：；“”？、—《》！（）-·</summary>
    [JsonPropertyName("profile")]
    public string? Profile { get; set; }
}
