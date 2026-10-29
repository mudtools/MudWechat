// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.ShopSpu;

/// <summary>商品草稿 / 审核状态（V2 <c>edit_status</c>；<c>get</c> 页枚举从 1 起，<c>get_list</c> 页含 0）。</summary>
public static class WxaShopSpuEditStatuses
{
    /// <summary>0 初始值（<c>get_list</c> 页枚举）。</summary>
    public const int Initial = 0;

    /// <summary>1 未审核（<c>get</c> 页枚举；<c>get_list</c> 页作「编辑中」）。</summary>
    public const int NotAudited = 1;

    /// <summary>2 审核中。</summary>
    public const int UnderReview = 2;

    /// <summary>3 审核失败。</summary>
    public const int ReviewFailed = 3;

    /// <summary>4 审核成功。</summary>
    public const int ReviewSucceeded = 4;
}
