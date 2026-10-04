// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Wedoc;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「文档」模块管理表格内容域公共 SDK
/// （编辑表格内容 + 获取表格数据 + 获取表格行列信息）。
/// <para>
/// 官方对三类应用开放完全一致的 3 个端点，全部收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 企业自建应用见 <see cref="IWechatWorkInternalWedocSpreadsheetService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyWedocSpreadsheetService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderWedocSpreadsheetService"/>。
/// </para>
/// <para>管理文档族见 <see cref="IWechatWorkWedocService"/>；
/// 管理文档内容族见 <see cref="IWechatWorkWedocDocumentService"/>。</para>
/// </summary>
/// <remarks>
/// <para>
/// 三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>），
/// 由多应用基座按当前应用上下文（AppKey + scope）路由——第三方/代开发消费授权企业级 access_token（scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。
/// </para>
/// <para>权限口径：企业自建应用需使用配置到「可调用应用」列表中的应用 secret 所获取的 access_token 调用；
/// 第三方应用与代开发自建应用需具有「文档」权限。编辑表格内容为批量更新形态：
/// 单次批量更新操作数量 &lt;= 5，各操作逐个按顺序执行，其中一个操作报错即不再执行后续操作。</para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkWedocSpreadsheetService
{
    /// <summary>
    /// 编辑表格内容
    /// <para>该接口用于批量编辑指定在线表格的内容（新增工作表 / 删除工作表 / 更新范围内单元格内容 /
    /// 删除连续的行或列）。</para>
    /// <para>官方业务限制：单次批量更新操作数量 &lt;= 5；各操作逐个按顺序执行，其中一个操作报错即不再执行后续操作，
    /// 每个操作执行前会做权限、参数等校验；新增工作表范围列数 &lt;= 200、范围内总单元格数量 &lt;= 10000；
    /// 更新范围行数 &lt;= 1000、列数 &lt;= 200、范围内总单元格数量 &lt;= 10000；
    /// 删除行列范围为左闭右开 [start_index, end_index)，若 end_index &lt;= start_index 则该请求报错（该操作会导致表格缩表）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="BatchUpdateWedocSpreadsheetRequest"/>：docid / requests）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>各操作对应的结果列表（data.responses，与请求操作一一对应）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101168"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101190"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/101169"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/spreadsheet/batch_update")]
    Task<BatchUpdateWedocSpreadsheetResponse> BatchUpdateSpreadsheetAsync(
        [Body] BatchUpdateWedocSpreadsheetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取表格数据
    /// <para>该接口用于获取指定范围内在线表格的数据（遵循 A1 表示法）。</para>
    /// <para>官方业务限制：查询范围行数 &lt;= 1000、列数 &lt;= 200、范围内总单元格数量 &lt;= 10000；
    /// range 遵循 A1 表示法（如 <c>A1:B5</c>），左上角单元格必须在右下角单元格左上方（<c>B5:A1</c> 不合法）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetWedocSpreadsheetDataRequest"/>：docid / sheet_id / range）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>表格数据（data.result，GridData）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97711"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98031"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98038"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/spreadsheet/get_sheet_range_data")]
    Task<GetWedocSpreadsheetDataResponse> GetSpreadsheetDataAsync(
        [Body] GetWedocSpreadsheetDataRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取表格行列信息
    /// <para>该接口用于获取在线表格的工作表属性列表（工作表 ID / 名称 / 总行数 / 总列数）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetWedocSpreadsheetPropertiesRequest"/>：docid）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>工作表属性列表（properties）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/97661"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98030"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98037"/></para>
    /// </remarks>
    [Post("/cgi-bin/wedoc/spreadsheet/get_sheet_properties")]
    Task<GetWedocSpreadsheetPropertiesResponse> GetSpreadsheetPropertiesAsync(
        [Body] GetWedocSpreadsheetPropertiesRequest request,
        CancellationToken cancellationToken = default);
}
