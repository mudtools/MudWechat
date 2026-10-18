// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram.DataModels.CityService;

/// <summary>获取城市服务路径类型（<c>page_type</c>）。</summary>
public static class WxaCityServicePageTypes
{
    /// <summary>0 服务主页（必填 service_id / params）。</summary>
    public const int ServiceHome = 0;

    /// <summary>1 首页（必填 city_name）。</summary>
    public const int Home = 1;

    /// <summary>3 专题页（必填 city_name / content_name）。</summary>
    public const int Topic = 3;

    /// <summary>5 服务列表页（必填 city_name / ext_params）。</summary>
    public const int ServiceList = 5;
}
