// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Mud.Wechat.Work.DataModels.Finance;
using Mud.Wechat.Work.ExtendedSDK.Finance;
using Mud.Wechat.Work.ExtendedSDK.Finance.InteropServices;
using Mud.Wechat.Work.Extensions;

namespace Mud.Wechat.Work.Tests.ContractGuards;

/// <summary>
/// 会话内容存档 C SDK 封装的契约守卫（FIN-B 系列）：原生封面对象性与成对释放、凭据面边界、
/// 分片聚合正确性、序列化纪律、明文/凭据不入诊断、与 HTTP 业务域面的零交叉。
/// </summary>
/// <remarks>
/// <para>
/// 本域是仓内<b>唯一</b>不经 HTTP 的能力面（原生 <c>WeWorkFinanceSdk</c> 进程内调用），故没有端点、
/// 路由与响应 DTO 可数 —— 守卫对象相应地落在「封面对象性 + 资源生命周期 + 凭据治理」三处，
/// 这三处正是原生封装最容易出事的地方（句柄泄漏、use-after-free、私钥进日志）。
/// </para>
/// <para>
/// <b>原生库不在场</b>：本守卫不加载 <c>WeWorkFinanceSdk.dll</c> / <c>libWeWorkFinanceSdk_C.so</c>，
/// 也不断言任何真机行为。凡需原生库才能验证的事实一律只作源码/反射断言（点名可见，不写成空断言）；
/// 聚合器的正确性用<b>假分片源</b>断言（FIN-B3），这也是把聚合逻辑抽成纯函数的原因。
/// </para>
/// </remarks>
public class WechatFinanceContractGuards
{
    /// <summary>官方 C 头文件声明的入口集（17 支）—— 封面对象性的权威清单。</summary>
    private static readonly string[] ExpectedNativeEntries =
    {
        "NewSdk", "Init", "GetChatData", "GetMediaData", "DecryptData",
        "DestroySdk", "NewSlice", "FreeSlice", "GetContentFromSlice", "GetSliceLen",
        "NewMediaData", "FreeMediaData", "GetOutIndexBuf", "GetData", "GetIndexLen",
        "GetDataLen", "IsMediaDataFinish",
    };

    /// <summary>FIN-B1：P/Invoke 覆盖面逐字锁定 —— 17 支、名称集合一致、全部 <c>CallingConvention.Cdecl</c>。</summary>
    /// <remarks>
    /// 名称清单是<b>集合相等</b>而非「包含」：多一支意味着有人补了未核验的入口（官方头文件之外），
    /// 少一支意味着封装面出现缺口。两者都必须显式改这份清单，从而在评审里可见。
    /// </remarks>
    [Fact]
    public void NativeEntries_ShouldMatchOfficialHeader_WhenFinanceInteropIsEnumerated()
    {
        var methods = typeof(WechatWorkFinanceClient).Assembly
            .GetType("Mud.Wechat.Work.ExtendedSDK.Finance.InteropServices.FinanceNativeMethods", throwOnError: true)!
            .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Select(static m => new { Method = m, Attr = m.GetCustomAttribute<DllImportAttribute>() })
            .Where(static x => x.Attr != null)
            .ToList();

        methods.Should().HaveCount(ExpectedNativeEntries.Length,
            $"原生入口须与官方 C 头文件声明的 {ExpectedNativeEntries.Length} 支一一对应");

        methods.Select(static x => x.Method.Name).OrderBy(static n => n, StringComparer.Ordinal)
            .Should().Equal(ExpectedNativeEntries.OrderBy(static n => n, StringComparer.Ordinal),
                "入口名逐字照抄官方（NewSdk 而非 SdkCreate 之类的美化改名会切断与 C 侧的对应关系）");

        methods.Should().OnlyContain(static x => x.Attr!.Value == FinanceNativeMethods.LibraryName,
            "库名一律用简单名（由 FinanceNativeLibrary 的解析器映射平台文件名）");
        methods.Should().OnlyContain(static x => x.Attr!.CallingConvention == CallingConvention.Cdecl,
            "官方 C 接口为 cdecl，写成 StdCall 在 x86 上会破坏栈平衡");

        // 参数类型逐字照抄官方 C 头：limit/timeout 漂移在 x64 上靠寄存器传参侥幸无害，
        // x86（.NET Framework 32 位宿主可跑）上会把栈读错位 —— 必须在类型层面锁死。
        var getChatData = methods.Single(static x => x.Method.Name == "GetChatData").Method;
        var getChatDataParams = getChatData.GetParameters();
        getChatDataParams.Should().HaveCount(7);
        getChatDataParams[1].ParameterType.Should().Be<ulong>("官方 seq 为 unsigned long long（uint64）");
        getChatDataParams[2].ParameterType.Should().Be<uint>("官方 limit 为 unsigned int（声明成 long 在 x86 上栈错位）");
        getChatDataParams[5].ParameterType.Should().Be<int>("官方 timeout 为 int");
        var getMediaData = methods.Single(static x => x.Method.Name == "GetMediaData").Method;
        var getMediaDataParams = getMediaData.GetParameters();
        getMediaDataParams.Should().HaveCount(7);
        getMediaDataParams[5].ParameterType.Should().Be<int>("官方 timeout 为 int");
    }

    /// <summary>FIN-B1b：三类原生句柄皆由 <c>SafeHandle</c> 承载且不公开（封送期引用计数 + 最终器兜底）。</summary>
    [Fact]
    public void NativeHandles_ShouldBeSafeHandleDerived_AndInternal()
    {
        var assembly = typeof(WechatWorkFinanceClient).Assembly;
        var handles = new[] { "FinanceSdkHandle", "FinanceSliceHandle", "FinanceMediaDataHandle" }
            .Select(n => assembly.GetType(
                $"Mud.Wechat.Work.ExtendedSDK.Finance.InteropServices.{n}", throwOnError: true)!);

        foreach (var handle in handles)
        {
            typeof(SafeHandle).IsAssignableFrom(handle).Should().BeTrue(
                $"{handle.Name} 必须走 SafeHandle：裸 IntPtr + 手写终结器无法防「原生调用进行中被回收」");
            handle.IsPublic.Should().BeFalse(
                $"{handle.Name} 不得进公开面：原生句柄是实现细节，宿主只经工厂取客户端");
        }

        // 成对释放：New/Init 侧与 Destroy/Free 侧一一对应，且都在 ReleaseHandle 里。
        var handlesSource = File.ReadAllText(SourcePath(
            "Mud.Wechat.Work", "ExtendedSDK", "Finance", "InteropServices", "FinanceNativeHandles.cs"));
        foreach (var pair in new[]
                 {
                     new[] { "NewSdk", "DestroySdk" },
                     new[] { "NewSlice", "FreeSlice" },
                     new[] { "NewMediaData", "FreeMediaData" },
                 })
        {
            handlesSource.Should().Contain($"{pair[0]}()", $"{pair[0]} 必须有创建入口");
            handlesSource.Should().Contain($"{pair[1]}(handle)",
                $"{pair[0]} 必须与 {pair[1]} 成对（ReleaseHandle 内调用），否则原生内存只增不减");
        }

        handlesSource.Should().Contain("return true",
            "ReleaseHandle 必须返回 true：返回 false 会让 SafeHandle 认为释放失败并反复重试");

        // 「用即释放」形态：写出缓冲一律 using，异常路径也释放。
        var clientSource = ReadClientSource();
        clientSource.Should().Contain("using var slice = FinanceSliceHandle.Create();",
            "Slice 必须 using 持有：判错/解析抛在写出之后，手写 FreeSlice 会漏在异常路径上");
        clientSource.Should().Contain("using var media = FinanceMediaDataHandle.Create();",
            "MediaData 同上");
    }

    /// <summary>FIN-B1c：两层判错各走各的异常类型，不得混用。</summary>
    /// <remarks>
    /// 原生返回码与信封 <c>errcode</c> 是两层不同事实：前者意味着「没有可用内容」，后者意味着
    /// 「内容有、业务不通过」。合用一种异常，调用方就无法判断该重试还是该换密钥。
    /// </remarks>
    [Fact]
    public void ErrorLayers_ShouldUseDistinctExceptionTypes()
    {
        typeof(WechatFinanceNativeException).BaseType.Should().Be<InvalidOperationException>(
            "原生层异常自成一型（继承 InvalidOperationException，不复用 HTTP 线异常基类）");
        typeof(WechatFinanceNativeException).IsAssignableFrom(
            typeof(Mud.Wechat.Work.Abstractions.Exceptions.WechatWorkException)).Should().BeFalse(
            "信封层判错走 WechatWorkException，两层不得同源");

        var clientSource = ReadClientSource();
        clientSource.Should().Contain("EnsureNativeSuccess(entryName, ret)",
            "原生返回码判错走单一咽喉点（非 0 即抛 WechatFinanceNativeException）");
        clientSource.Should().Contain("WechatWorkException.ThrowIfFailed(envelope)",
            "信封 errcode 判错走企微统一出口");

        // 取数阶段不重试（游标语义下重试与「下一次调用」等价，交上层轮询）：
        // 重试判据只允许出现在聚合器与码常量两处的**代码**里（文档里的 cref 引用不算调用点）。
        var retrySites = FinanceSourceFiles()
            .Where(static f => CodeLines(f).Any(static l =>
                l.Contains("IsMediaShardRetryable", StringComparison.Ordinal)))
            .Select(static f => Path.GetFileName(f))
            .OrderBy(static n => n, StringComparer.Ordinal)
            .ToList();
        retrySites.Should().Equal(new[] { "FinanceMediaChunk.cs", "WechatFinanceNativeException.cs" },
            "媒体分片重试判据只在聚合器与其定义处出现；GetChatData/DecryptData 不得挂重试");
    }

    /// <summary>FIN-B2：凭据面边界 —— 配置只有「密钥名」，实例上不留私钥/secret。</summary>
    [Fact]
    public void Credentials_ShouldNeverBeDataModelFields_AndClientKeepsNoKeyMaterial()
    {
        var credentialishPropertyNames = new[]
        {
            nameof(WechatFinanceRobotOptions.SecretSecretName),
            nameof(WechatFinanceRobotOptions.ProxyPasswordSecretName),
            nameof(WechatFinanceRobotOptions.PrivateKeySecretNames),
        };

        foreach (var optionsType in new[] { typeof(WechatFinanceOptions), typeof(WechatFinanceRobotOptions) })
        {
            foreach (var property in optionsType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var looksLikeCredential = property.Name.Contains("Secret", StringComparison.Ordinal)
                                          || property.Name.Contains("PrivateKey", StringComparison.Ordinal)
                                          || property.Name.Contains("Password", StringComparison.Ordinal);
                if (looksLikeCredential)
                {
                    credentialishPropertyNames.Should().Contain(property.Name,
                        $"{optionsType.Name}.{property.Name} 必须形如「*SecretName」——配置面只登记密钥名，永不承载密钥值");
                }
            }

            optionsType.GetProperties().Should().NotContain(static p => p.PropertyType.Name.Contains("RSA"),
                $"{optionsType.Name} 不得出现 RSA/密钥对象类型的属性");
        }

        // 客户端实例字段：字符串字段只允许机器人键、代理地址与代理口令（后者每次调用都要传，是唯一驻留的敏感串）。
        var stringFields = typeof(WechatWorkFinanceClient)
            .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
            // 可空标注在运行时被擦除 ⇒ string? 字段的 FieldType 同为 typeof(string)。
            .Where(static f => f.FieldType == typeof(string))
            .Select(static f => f.Name)
            .OrderBy(static n => n, StringComparer.Ordinal)
            .ToList();
        stringFields.Should().Equal(new[] { "_proxyAddress", "_proxyPassword", "_robotKey" },
            "客户端不得驻留 corpid / 存档 secret / RSA 私钥（私钥现取现用，corpid 与 secret 已交予原生 Init）");

        typeof(WechatWorkFinanceClient)
            .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
            .Should().NotContain(static f => f.FieldType.Name.Contains("RSA"),
                "私钥不得作为字段缓存（每次解密经 ISecretProvider 取用）");

        // 启动期拒绝「把 PEM 贴进配置」（行为断言见 WechatFinanceOptionsTests；此处锁判据存在）。
        File.ReadAllText(SourcePath("Mud.Wechat.Work", "ExtendedSDK", "Finance", "WechatFinanceOptions.cs"))
            .Should().Contain("\"-----BEGIN\"",
                "必须显式识别 PEM 头，否则误填私钥原文会把密钥洒进「查无密钥名」的异常消息");
    }

    /// <summary>FIN-B3a：多片按游标续传、顺序拼接、以完成标记收尾。</summary>
    [Fact]
    public async Task Aggregate_ShouldFollowCursors_AndStopOnFinish()
    {
        var cursors = new List<string?>();
        var shards = new Queue<FinanceMediaChunk>(new[]
        {
            Shard("aa", "c1", finished: false),
            Shard("bb", "c2", finished: false),
            Shard("cc", null, finished: true),
        });

        var bytes = await FinanceMediaAssembler.AggregateAsync(
            cursor =>
            {
                cursors.Add(cursor);
                return Task.FromResult(shards.Dequeue());
            },
            retryCount: 0,
            retryDelayMs: 0,
            CancellationToken.None);

        Encoding.UTF8.GetString(bytes).Should().Be("aabbcc", "拼接顺序即分片顺序（乱序即文件损坏）");
        cursors.Should().Equal(new string?[] { null, "c1", "c2" },
            "首片游标为 null，其后逐片使用上一次回写的游标");
        shards.Should().BeEmpty("完成标记之后不得再取片（多取一次等于用空游标重下）");
    }

    /// <summary>FIN-B3b：512KB 官方单片上限处的边界（两片各 512KB ⇒ 恰 1MB，无丢尾、无重复）。</summary>
    [Fact]
    public async Task Aggregate_ShouldKeepBothShards_AtOfficialShardBoundary()
    {
        WechatFinanceOptions.OfficialMediaShardBytes.Should().Be(512 * 1024,
            "单片上限是官方常量（仅诊断/预估用，不据此截断数据）");

        var first = new byte[WechatFinanceOptions.OfficialMediaShardBytes];
        var second = new byte[WechatFinanceOptions.OfficialMediaShardBytes];
        Array.Fill(first, byte.MaxValue);
        Array.Fill(second, (byte)7);
        second[second.Length - 1] = 9; // 尾字节：最容易被截断的位置

        var queue = new Queue<FinanceMediaChunk>(new[]
        {
            new FinanceMediaChunk(first, "c2", false),
            new FinanceMediaChunk(second, null, true),
        });

        var bytes = await FinanceMediaAssembler.AggregateAsync(
            _ => Task.FromResult(queue.Dequeue()), 0, 0, CancellationToken.None);

        bytes.Should().HaveCount(first.Length + second.Length, "两片完整拼接（截断=文件不可用）");
        bytes[0].Should().Be(byte.MaxValue);
        bytes[bytes.Length - 1].Should().Be(9, "末片尾字节必须保留");
        bytes[first.Length].Should().Be(7, "第二片首字节不得与第一片重叠");
    }

    /// <summary>FIN-B3c：同游标退避重试 —— 重试<b>不推进游标</b>，成功后计数归零。</summary>
    [Fact]
    public async Task Aggregate_ShouldRetrySameCursor_AndResetAfterSuccess()
    {
        var cursors = new List<string?>();
        var failed = new HashSet<string?>(StringComparer.Ordinal);

        var bytes = await FinanceMediaAssembler.AggregateAsync(
            cursor =>
            {
                cursors.Add(cursor);

                // 每片「第一次」都失败（两片 ⇒ 两次失败），第二次成功。
                // retryCount 恰为 1 ⇒ 只有在「成功一片后计数归零」时整体才可能成功。
                if (!failed.Contains(cursor))
                {
                    failed.Add(cursor);
                    return Task.FromException<FinanceMediaChunk>(
                        new WechatFinanceNativeException("GetMediaData", 10001, "分片失败（测试）"));
                }

                return Task.FromResult(cursor == null
                    ? Shard("head", "c1", false)
                    : Shard("tail", null, true));
            },
            retryCount: 1,
            retryDelayMs: 0,
            CancellationToken.None);

        Encoding.UTF8.GetString(bytes).Should().Be("headtail");
        cursors.Should().Equal(new string?[] { null, null, "c1", "c1" },
            "重试必须打在同一个游标上：推进游标等于跳过失败的那一片，拼出来的是残缺文件");
    }

    /// <summary>FIN-B3d：重试上限用尽后原样上抛；关闭重试（0 次）时立即上抛。</summary>
    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    public async Task Aggregate_ShouldSurfaceNativeError_WhenRetryBudgetExhausted(int retryCount)
    {
        var calls = 0;

        var act = () => FinanceMediaAssembler.AggregateAsync(
            _ =>
            {
                calls++;
                return Task.FromException<FinanceMediaChunk>(
                    new WechatFinanceNativeException("GetMediaData", 10002, "持续失败（测试）"));
            },
            retryCount,
            0,
            CancellationToken.None);

        (await act.Should().ThrowAsync<WechatFinanceNativeException>())
            .Which.ReturnCode.Should().Be(10002, "上抛的是原生层异常（不被包装成 InvalidOperationException，调用方仍可按码判定）");
        calls.Should().Be(retryCount + 1, "首取 + retryCount 次重试，不许多也不少");
    }

    /// <summary>FIN-B3e：不可重试的返回码直接上抛（不把「权限/参数类故障」伪装成可恢复抖动）。</summary>
    [Fact]
    public async Task Aggregate_ShouldNotRetry_NonRetryableNativeCode()
    {
        var calls = 0;

        var act = () => FinanceMediaAssembler.AggregateAsync(
            _ =>
            {
                calls++;
                return Task.FromException<FinanceMediaChunk>(
                    new WechatFinanceNativeException("GetMediaData", -1, "不可重试（测试）"));
            },
            retryCount: 5,
            0,
            CancellationToken.None);

        await act.Should().ThrowAsync<WechatFinanceNativeException>();
        calls.Should().Be(1);
    }

    /// <summary>FIN-B3f：「未完成却无游标」与「0 字节却未完成」都中止，而不是从头重复拼接。</summary>
    [Fact]
    public async Task Aggregate_ShouldAbort_WhenProgressCannotAdvance()
    {
        var act1 = () => FinanceMediaAssembler.AggregateAsync(
            _ => Task.FromResult(Shard("xx", null, finished: false)), 0, 0, CancellationToken.None);
        (await act1.Should().ThrowAsync<InvalidOperationException>())
            .Which.Message.Should().Contain("未回写续传游标",
                "未完成且无游标时唯一可行的继续是「重下同一片」，那会把内容重复拼进文件");

        var act2 = () => FinanceMediaAssembler.AggregateAsync(
            _ => Task.FromResult(new FinanceMediaChunk(Array.Empty<byte>(), "c1", false)), 0, 0, CancellationToken.None);
        (await act2.Should().ThrowAsync<InvalidOperationException>())
            .Which.Message.Should().Contain("无进度",
                "0 字节 + 有游标 + 未完成 = 原地不动，逐片循环永不收敛");
    }

    /// <summary>FIN-B3g：取消只在分片边界生效（已进入的原生调用不可中断，这是文档承诺而非实现巧合）。</summary>
    [Fact]
    public async Task Aggregate_ShouldHonourCancellation_BetweenShards()
    {
        using var cts = new CancellationTokenSource();

        var act = () => FinanceMediaAssembler.AggregateAsync(
            _ =>
            {
                cts.Cancel();
                return Task.FromResult(Shard("xx", "c1", false));
            },
            0,
            0,
            cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    /// <summary>FIN-B4a：Finance 上下文已生成并登记三支根类型（AOT 源生成，非反射）。</summary>
    [Fact]
    public void FinanceJsonContext_ShouldRegisterRootTypes()
    {
        var context = FinanceJsonContext.Default;

        foreach (var type in new[]
                 {
                     typeof(FinanceChatDataEnvelope),
                     typeof(FinanceChatDataRow),
                     typeof(FinanceChatMessage),
                 })
        {
            context.GetTypeInfo(type).Should().NotBeNull(
                $"{type.Name} 必须由源生成上下文登记（反射版 JsonSerializer 是 AOT 红线）");
        }
    }

    /// <summary>FIN-B4b：JSON 解析走 JsonTypeInfo；反射版只允许出现在低 TFM 条件分支且带 pragma。</summary>
    [Fact]
    public void JsonParsing_ShouldUseSourceGeneratedContextOnNet8Path()
    {
        var source = ReadClientSource();
        var net8Path = source[..source.IndexOf("#else", StringComparison.Ordinal)];

        net8Path.Should().Contain("JsonSerializer.Deserialize(json, FinanceJsonContext.Default.FinanceChatDataEnvelope)");
        net8Path.Should().Contain("JsonSerializer.Deserialize(json, FinanceJsonContext.Default.FinanceChatMessage)");
        net8Path.Should().NotContain("JsonSerializer.Deserialize<",
            "net8 分支不得用泛型反射重载（IL2026/IL3050 门禁只在此档生效）");

        // 低 TFM 分支必须显式收敛诊断并说明成因（*JsonContext 本体是 NET8_0_OR_GREATER）。
        var lowTfm = source[source.IndexOf("#else", StringComparison.Ordinal)..];
        lowTfm.Should().Contain("#pragma warning disable IL2026", "低 TFM 反射反序列化须点名抑制并附理由");
        lowTfm.Should().Contain("#pragma warning disable IL3050", "同上（AOT 下动态代码生成）");
        lowTfm.Should().Contain("PropertyNameCaseInsensitive = true",
            "低 TFM 选项口径必须与生成的 FinanceJsonContext 一致，否则跨 TFM 解析行为漂移");

        // 设备控制符只在异常路径上清洗一次（先原样解析）。
        source.Should().Contain("catch (JsonException ex)", "清洗必须发生在解析失败之后");
        // 清洗对象是「原始 U+0011 字符」的源码形态（单反斜杠转义），而不是 6 字符转义文本 "\\u0011"：
        // 后者是合法 JSON、解析不会失败，剥它对 JsonException 路径永不生效（行为级验证见
        // WechatWorkFinanceClientParseTests）。
        source.Should().Contain("\"\\u0011\"", "官方脏数据形态：原始 U+0011~U+0014 控制字节（单反斜杠转义的字符字面量）");
        source.Should().NotContain("\\\\u0011",
            "6 字符转义文本形态（源码双反斜杠）是永不生效的死分支（合法 JSON 不进 JsonException 路径），出现即回归");
    }

    /// <summary>FIN-B4c：不使用 <c>GetDelegateForFunctionPointer</c>（IL3050 红线），也不依赖 NUL 结尾。</summary>
    [Fact]
    public void Interop_ShouldNotUseFunctionPointerMarshalling()
    {
        foreach (var file in FinanceSourceFiles())
        {
            var code = string.Join("\n", CodeLines(file));
            code.Should().NotContain("GetDelegateForFunctionPointer",
                $"{Path.GetFileName(file)}：该入口带 [RequiresDynamicCode]，会打穿 IL3050 净零门禁");
            code.Should().NotContain("PtrToStringUTF8",
                $"{Path.GetFileName(file)}：netstandard2.0 无此入口，且原生侧本就提供长度入口");
            code.Should().NotContain("SetLastError = true",
                $"{Path.GetFileName(file)}：官方 C 接口不回写 Win32 LastError，开了只会误导排查");
        }
    }

    /// <summary>FIN-B5a：Finance 源码零日志器 —— 明文即敏感，SDK 侧不提供「顺手打日志」的通道。</summary>
    [Fact]
    public void Finance_ShouldNotUseLoggers()
    {
        foreach (var file in FinanceSourceFiles())
        {
            var text = File.ReadAllText(file);
            foreach (var token in new[] { "ILogger", "LogDebug", "LogInformation", "LogWarning", "LogError", "_logger" })
            {
                text.Should().NotContain(token,
                    $"{Path.GetFileName(file)}：会话正文/凭据一律不得进日志（诊断只经异常消息）");
            }
        }
    }

    /// <summary>FIN-B5b：凭据与报文内容不得进入字符串插值（诊断只允许密钥名的截断形态）。</summary>
    [Fact]
    public void Finance_ShouldNotInterpolateSecretsOrPayloads()
    {
        var forbidden = new[]
        {
            "secret", "privateKeyPem", "encryptKey", "EncryptChatMsg", "EncryptRandomKey",
            "CorpId", "ContentJson", "ProxyPassword",
        };

        foreach (var file in FinanceSourceFiles())
        {
            var lines = File.ReadAllLines(file);
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                // 只看插值孔里的标识符：{secret} / {robot.CorpId} / {row.EncryptChatMsg} 这类形态。
                foreach (var hole in InterpolationHoles(line))
                {
                    foreach (var name in forbidden)
                    {
                        hole.Should().NotMatchRegex($@".*\b{name}\b.*",
                            $"{Path.GetFileName(file)}:{i + 1} 把「{name}」拼进了诊断文本（异常消息/字符串），" +
                            "凭据与正文一律不得外泄；密钥名请用 Truncate(...) 承载");
                    }
                }
            }
        }

        // 反向确认「允许且必要」的截断形态存在（避免上面的扫描被误读成「一切都已检查」）。
        ReadClientSource().Should().Contain("Truncate(secretName!)");
        File.ReadAllText(SourcePath("Mud.Wechat.Work", "ExtendedSDK", "Finance", "WechatFinanceClientFactory.cs"))
            .Should().Contain("Truncate(secretName)");
    }

    /// <summary>FIN-B5c：私钥解析失败的回执不含 PEM 内容，只给机器人键与版本号。</summary>
    [Fact]
    public void Cipher_ShouldNotLeakKeyMaterial_WhenParsingFails()
    {
        const string fakePem = "-----BEGIN PRIVATE KEY-----\nMIIleakcanaryvalue\n-----END PRIVATE KEY-----";
        const string canary = "leakcanaryvalue";

        // 密文给合法 base64：让失败点落在 ImportFromPem（唯一有回显风险的分支）。
        var encryptRandomKey = Convert.ToBase64String(new byte[] { 1, 2, 3, 4 });

        var act = () => FinanceCipher.DecryptRandomKey(fakePem, encryptRandomKey, "robot-1", 1);

        var ex = act.Should().Throw<InvalidOperationException>();
        ex.Which.Message.Should().NotContain(canary, "PEM 内容不得进异常消息（含密钥体）");
        ex.Which.Message.Should().Contain("robot-1").And.Contain("publickey_ver=1",
            "定位信息只给机器人键与公钥版本号");
    }

    /// <summary>FIN-B6：与 HTTP 业务域面零交叉 —— 不进模块枚举、不是 [HttpClientApi]、注册入口独立。</summary>
    [Fact]
    public void Finance_ShouldStayOutOfHttpModuleSurface()
    {
        Enum.GetNames(typeof(WechatModule))
            .Should().NotContain(n => n.Contains("Finance", StringComparison.OrdinalIgnoreCase),
                "会话存档是原生封装，不是 HTTP 域 ⇒ 不得进 WechatModule（模块面由 N1~N3 与模块一致性守卫锁定）");

        typeof(IWechatWorkFinanceClient)
            .GetCustomAttributes(typeof(Mud.HttpUtils.Attributes.HttpClientApiAttribute), true)
            .Should().BeEmpty("非 [HttpClientApi]：本域没有路由，挂上去会让源生成器造出打不通的客户端");

        typeof(IWechatWorkFinanceClient).GetInterfaces().Should().BeEmpty(
            "不继承任何业务接口基座（令牌链路对本域无意义）");

        var registrations = typeof(WechatFinanceServiceCollectionExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(static m => m.Name == "AddWechatFinanceSdk")
            .ToList();
        registrations.Should().HaveCount(2, "配置节版 + 代码配置版两个重载");
        registrations.Should().OnlyContain(static m => m.ReturnType == typeof(IServiceCollection));
        WechatFinanceOptions.DefaultSectionName.Should().Be("WechatFinance");

        // 注册入口不落 WechatWorkServiceBuilder（Add{域}Api 面与本域无交叉）。
        File.ReadAllText(SourcePath("Mud.Wechat.Work", "Extensions", "WechatWorkServiceBuilder.cs"))
            .Should().NotContain("Finance", "AddWechatFinanceSdk 是独立入口，不进 Add{域}Api 建造者");

        // 原生库不随包分发，且不进组件 HTTP 管线 ⇒ 不登记 SSRF 白名单（AB-G4 单点约束）。
        FinanceSourceFiles().Should().NotContain(static f =>
            File.ReadAllText(f).Contains("AddWechatApiHosts", StringComparison.Ordinal));
    }

    private static string ReadClientSource()
        => File.ReadAllText(SourcePath("Mud.Wechat.Work", "ExtendedSDK", "Finance", "WechatWorkFinanceClient.cs"));

    /// <summary>取代码行（剔除 <c>///</c> 文档注释行）—— 禁令断言的判据是调用点，本域 remarks 里逐条写着「为什么不用 X」。</summary>
    private static IEnumerable<string> CodeLines(string file)
        => File.ReadLines(file).Where(static line =>
            !line.TrimStart().StartsWith("///", StringComparison.Ordinal));

    private static IEnumerable<string> FinanceSourceFiles()
        => Directory.EnumerateFiles(
                Path.Combine(SourceProjectDir("Mud.Wechat.Work"), "ExtendedSDK", "Finance"),
                "*.cs", SearchOption.AllDirectories)
            .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal));

    private static FinanceMediaChunk Shard(string content, string? nextIndexBuffer, bool finished)
        => new(Encoding.UTF8.GetBytes(content), nextIndexBuffer, finished);

    /// <summary>取出一行里所有 <c>$"...{hole}..."</c> 的插值孔内容（粗粒度但足以捕捉标识符）。</summary>
    private static IEnumerable<string> InterpolationHoles(string line)
    {
        var index = 0;
        while (index < line.Length)
        {
            var start = line.IndexOf('{', index);
            if (start < 0 || start + 1 >= line.Length || line[start + 1] == '{')
            {
                break;
            }

            var end = line.IndexOf('}', start + 1);
            if (end < 0)
            {
                break;
            }

            yield return line.Substring(start + 1, end - start - 1);
            index = end + 1;
        }
    }

    private static string GetSolutionRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory != null && !File.Exists(Path.Combine(directory.FullName, "Mud.Wechat.slnx")))
        {
            directory = directory.Parent!;
        }

        return directory!.FullName;
    }

    private static string SourcePath(params string[] segments) =>
        Path.Combine(new[] { SourceProjectDir(segments[0]) }.Concat(segments.Skip(1)).ToArray());

    private static string SourceProjectDir(string projectName) =>
        SourceProjectDirs.GetOrAdd(projectName, static name =>
            Directory.EnumerateFiles(GetSolutionRoot(), $"{name}.csproj", SearchOption.AllDirectories)
                .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                   && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                .Select(static f => Path.GetDirectoryName(f))!
                .FirstOrDefault()
            ?? throw new DirectoryNotFoundException($"未找到工程 {name}.csproj（源码归类目录漂移，守卫定位失效）"));

    private static readonly ConcurrentDictionary<string, string> SourceProjectDirs = new();
}
