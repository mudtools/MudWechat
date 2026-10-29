// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Window;

/// <summary>团购优惠券码来源常量（官方 <c>code_source_type</c> 枚举值；创建后不支持修改）。</summary>
public static class ChannelsLocallifeCodeSourceTypes
{
    /// <summary>平台生成。</summary>
    public const int Platform = 1;

    /// <summary>商家预存 code（官方 add 页原文「商家预存code，用户支付成功后平台发放」；update/get 页作「商家自定义」）。</summary>
    public const int Merchant = 2;

    /// <summary>发券实时传 code（官方注明「实时发券模式暂不开放」）。</summary>
    public const int Realtime = 3;
}
