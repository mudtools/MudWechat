// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount.DataModels.Store;

/// <summary>
/// 修改门店小程序信息（<c>POST /wxa/modify_merchant</c>）请求体。
/// </summary>
/// <remarks>
/// <para>
/// 官方字段表：<c>headimg_mediaid</c> / <c>intro</c>；两字段均标「必填」但说明为「不改可传空值」
/// ——必填性与可空语义矛盾（照录，SDK 以可空承载）。
/// </para>
/// <para>官方错误码：<c>40001</c>（本页错误码表仅此行，照录）。</para>
/// </remarks>
[HttpJsonSerializable(SerializerClassName = "Store")]
public class MpModifyMerchantRequest
{
    /// <summary>获取或设置头像临时素材 mediaid（官方 <c>headimg_mediaid</c>；不改可传空值）。</summary>
    [JsonPropertyName("headimg_mediaid")]
    public string? HeadImageMediaId { get; set; }

    /// <summary>获取或设置门店小程序介绍（官方 <c>intro</c>；不改可传空值）。</summary>
    [JsonPropertyName("intro")]
    public string? Intro { get; set; }
}
