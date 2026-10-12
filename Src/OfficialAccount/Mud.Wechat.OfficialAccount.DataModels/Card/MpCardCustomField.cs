// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Card;

// ---------------------------------------------------------------- 会员卡 / 礼品卡子表

/// <summary>自定义会员信息类目（官方 <c>custom_field1..3</c>）—— 礼品卡与会员卡<b>同表</b>，建卡与修改共用一型。</summary>
[HttpJsonSerializable(SerializerClassName = "Card")]
public class MpCardCustomField
{
    /// <summary>获取或设置类目名称类型（官方 <c>name_type</c>，会员信息类目模板名）。</summary>
    [JsonPropertyName("name_type")]
    public string? NameType { get; set; }

    /// <summary>获取或设置类目自定义名称（官方 <c>name</c>，4 字符以内）。</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>获取或设置类目跳转外链（官方 <c>url</c>）。</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
