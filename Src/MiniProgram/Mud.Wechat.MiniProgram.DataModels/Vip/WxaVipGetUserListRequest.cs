// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.Vip;

/// <summary>获取小程序会员列表请求体（<c>POST /wxa/vip/user/list/get</c>）。</summary>
/// <remarks>官方文档：<c>https://developers.weixin.qq.com/doc/store/shop/API/wxamember/wxa/api_v3_wxa_getuserlist</c>。</remarks>
[HttpJsonSerializable(SerializerClassName = "Vip")]
public class WxaVipGetUserListRequest
{
    /// <summary>偏移量（<c>offset</c>，必填；官方类型 number）。</summary>
    [JsonPropertyName("offset")]
    public long? Offset { get; set; }

    /// <summary>本次读取数量（<c>limit</c>，必填；官方类型 number）。</summary>
    [JsonPropertyName("limit")]
    public long? Limit { get; set; }
}
