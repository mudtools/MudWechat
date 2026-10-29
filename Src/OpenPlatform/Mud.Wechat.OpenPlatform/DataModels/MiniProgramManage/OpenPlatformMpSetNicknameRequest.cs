// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.MiniProgramManage;

/// <summary>
/// 「修改小程序名字」（<c>setnickname</c>）请求。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/miniprogram-management/basic-info-management/setNickName.html"/>。
/// </summary>
/// <remarks>官方约束：名称唯一、需符合命名规范，命中敏感/资质场景须提供 <c>id_card</c> / <c>license</c> / <c>naming_other_stuff_*</c> 材料。</remarks>
[HttpJsonSerializable(SerializerClassName = "MiniProgramManage")]
public class OpenPlatformMpSetNicknameRequest
{
    /// <summary>新名称（官方必填 <c>nick_name</c>，string）。</summary>
    [JsonPropertyName("nick_name")]
    public string? NickName { get; set; }

    /// <summary>身份证材料（官方选填 <c>id_card</c>，string）。</summary>
    [JsonPropertyName("id_card")]
    public string? IdCard { get; set; }

    /// <summary>营业执照 / 资质材料（官方选填 <c>license</c>，string）。</summary>
    [JsonPropertyName("license")]
    public string? License { get; set; }

    /// <summary>其他辅助材料 1（官方选填 <c>naming_other_stuff_1</c>）。</summary>
    [JsonPropertyName("naming_other_stuff_1")]
    public string? NamingOtherStuff1 { get; set; }

    /// <summary>其他辅助材料 2（官方选填 <c>naming_other_stuff_2</c>）。</summary>
    [JsonPropertyName("naming_other_stuff_2")]
    public string? NamingOtherStuff2 { get; set; }

    /// <summary>其他辅助材料 3（官方选填 <c>naming_other_stuff_3</c>）。</summary>
    [JsonPropertyName("naming_other_stuff_3")]
    public string? NamingOtherStuff3 { get; set; }

    /// <summary>其他辅助材料 4（官方选填 <c>naming_other_stuff_4</c>）。</summary>
    [JsonPropertyName("naming_other_stuff_4")]
    public string? NamingOtherStuff4 { get; set; }

    /// <summary>其他辅助材料 5（官方选填 <c>naming_other_stuff_5</c>）。</summary>
    [JsonPropertyName("naming_other_stuff_5")]
    public string? NamingOtherStuff5 { get; set; }
}
