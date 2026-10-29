// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Window;

/// <summary>
/// 团购优惠商品资质（官方 <c>product_qua_infos[]</c> 元素；add/update 请求与 get 响应共用——字段集一致。
/// 官方 add 页参数表 qua_id 为 number、update 页为 string——矛盾照录，按 add 页与 get 响应表建模为数字）。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Window")]
public class ChannelsLocallifeProductQuaInfo
{
    /// <summary>获取或设置商品资质 ID（官方 <c>qua_id</c>；对应类目信息中 product_qua_list[].qua_id）。</summary>
    [JsonPropertyName("qua_id")]
    public long? QuaId { get; set; }

    /// <summary>获取或设置商品资质图片列表（官方 <c>qua_url</c>；单个资质 ID 下最多 10 张，须使用「上传图片」接口）。</summary>
    [JsonPropertyName("qua_url")]
    public List<string>? QuaUrl { get; set; }
}
