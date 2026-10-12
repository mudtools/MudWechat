// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Brand;

/// <summary>更新品牌资质（<c>updatebrandlogic</c>）请求体。</summary>
/// <remarks>官方契约：<b>POST</b> + 请求体 <c>{"brand":{…}}</c>；调用即自动提交审核。</remarks>
[HttpJsonSerializable(SerializerClassName = "Brand")]
public class ChannelsUpdateBrandRequest
{
    /// <summary>获取或设置品牌详情（官方 <c>brand</c>，必填；与新增请求的 brand 对象字段集一致 ⇒ 共用 <see cref="ChannelsBrandDetail"/>）。</summary>
    [JsonPropertyName("brand")]
    public ChannelsBrandDetail Brand { get; set; } = new();
}
