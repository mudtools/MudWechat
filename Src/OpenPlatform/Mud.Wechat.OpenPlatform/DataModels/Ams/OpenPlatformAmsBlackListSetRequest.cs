// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OpenPlatform.DataModels.Ams;

/// <summary>
/// 「设置广告黑名单」请求（<c>agency_set_black_list</c>）。
/// 官方文档：<see href="https://developers.weixin.qq.com/doc/oplatform/openApi/OpenApiDoc/ams/ad-black/SetBlackList.html"/>。
/// </summary>
[HttpJsonSerializable(SerializerClassName = "Ams")]
public class OpenPlatformAmsBlackListSetRequest
{
    /// <summary>操作类型（官方必填 <c>op</c>，number：1 设置屏蔽，2 删除屏蔽）。</summary>
    [JsonPropertyName("op")]
    public long Op { get; set; }

    /// <summary>
    /// 屏蔽列表（官方必填 <c>list</c>，string，为 JSON 序列化字符串，含屏蔽类型 <c>type</c> 及标识 <c>id</c>；
    /// type 枚举：1 公众号 / 2 iOS 应用 / 3 安卓应用 / 4 小程序小游戏）。官方对各类型屏蔽数量设上限。
    /// </summary>
    [JsonPropertyName("list")]
    public string? List { get; set; }
}
