// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Resource;

/// <summary>图片返回数据类型常量（官方 <c>resp_type</c> URL 参数枚举值，<c>img_upload</c>）。</summary>
public static class ChannelsResourceImgRespTypes
{
    /// <summary>media_id 和 pay_media_id（大小限制 2MB）。</summary>
    public const int MediaId = 0;

    /// <summary>图片链接（形如 mmecimage.cn/p/{appid}/{imgKey}，永久有效；官方原文「商品信息相关图片请务必使用此参数得到链接」；大小限制 10MB）。</summary>
    public const int Url = 1;
}
