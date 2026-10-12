// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Channels.DataModels.Live;

/// <summary>直播大屏画像维度常量（官方 DimensionType 枚举值，<c>getlivedata</c> 维度组合键的组成维度）。</summary>
public static class ChannelsLiveDimensionTypes
{
    /// <summary>一级渠道。</summary>
    public const int PrimaryChannel = 1;

    /// <summary>年龄段。</summary>
    public const int AgeGroup = 2;

    /// <summary>性别。</summary>
    public const int Gender = 3;

    /// <summary>关注关系（画像侧）。</summary>
    public const int FollowRelation = 5;

    /// <summary>二级渠道。</summary>
    public const int SecondaryChannel = 7;

    /// <summary>策略人群。</summary>
    public const int StrategyAudience = 9;

    /// <summary>省级行政区。</summary>
    public const int Province = 10;

    /// <summary>地级行政区。</summary>
    public const int City = 11;

    /// <summary>消费者商品类目偏好。</summary>
    public const int CategoryPreference = 12;

    /// <summary>客单价区间。</summary>
    public const int PriceRange = 13;

    /// <summary>关注关系。</summary>
    public const int FollowRelation2 = 15;

    /// <summary>流量类型（自然流量、直播加热、广告投流）。</summary>
    public const int TrafficType = 16;
}
