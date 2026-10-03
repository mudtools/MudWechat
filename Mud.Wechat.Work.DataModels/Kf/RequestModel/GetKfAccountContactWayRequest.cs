// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Work.DataModels.Kf;

/// <summary>
/// 获取客服账号链接请求体（<c>/cgi-bin/kf/add_contact_way</c>）。
/// <para>
/// 返回的客服链接可嵌入 H5 页面或据此生成二维码；
/// 链接不能修改或复制参数到其他链接使用，否则进入会话事件参数校验不通过，导致无法回调。
/// </para>
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Kf")]
public class GetKfAccountContactWayRequest
{
    /// <summary>
    /// 获取或设置客服账号 ID（官方必填）。
    /// </summary>
    [JsonPropertyName("open_kfid")]
    public string? OpenKfId { get; set; }

    /// <summary>
    /// 获取或设置自定义场景值（不多于 32 个字节，取值需匹配正则 <c>[0-9a-zA-Z_-]*</c>）。
    /// <para>
    /// scene 非空时，返回链接可拼接 <c>scene_param=SCENE_PARAM</c> 参数使用，用户进入会话事件会原样返回该值；
    /// SCENE_PARAM 需 urlencode，编码前长度不超过 128 个字节。
    /// </para>
    /// </summary>
    [JsonPropertyName("scene")]
    public string? Scene { get; set; }
}
