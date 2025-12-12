using Microsoft.JSInterop;
using System.Text.Json;

namespace ReportBuilder.Services;

/// <summary>
/// Service for storing and retrieving data in browser localStorage
/// </summary>
public class LocalStorageService
{
    private readonly IJSRuntime _jsRuntime;

    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Save iOS app identifier to localStorage
    /// </summary>
    public async Task SaveIOSAppIdentifierAsync(string appId)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "iosAppIdentifier", appId);
    }

    /// <summary>
    /// Get iOS app identifier from localStorage
    /// </summary>
    public async Task<string?> GetIOSAppIdentifierAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "iosAppIdentifier");
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Save Android account identifier to localStorage
    /// </summary>
    public async Task SaveAndroidAccountIdentifierAsync(string accountId)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "androidAccountIdentifier", accountId);
    }

    /// <summary>
    /// Get Android account identifier from localStorage
    /// </summary>
    public async Task<string?> GetAndroidAccountIdentifierAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "androidAccountIdentifier");
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Save Android app identifier to localStorage
    /// </summary>
    public async Task SaveAndroidAppIdentifierAsync(string appId)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "androidAppIdentifier", appId);
    }

    /// <summary>
    /// Get Android app identifier from localStorage
    /// </summary>
    public async Task<string?> GetAndroidAppIdentifierAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "androidAppIdentifier");
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Clear iOS app identifier from localStorage
    /// </summary>
    public async Task ClearIOSAppIdentifierAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "iosAppIdentifier");
    }

    /// <summary>
    /// Clear Android account identifier from localStorage
    /// </summary>
    public async Task ClearAndroidAccountIdentifierAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "androidAccountIdentifier");
    }

    /// <summary>
    /// Clear Android app identifier from localStorage
    /// </summary>
    public async Task ClearAndroidAppIdentifierAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "androidAppIdentifier");
    }

    /// <summary>
    /// Save report config to cache
    /// </summary>
    public async Task SaveReportCacheAsync(string configJson)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "reportCache", configJson);
    }

    /// <summary>
    /// Get report config from cache
    /// </summary>
    public async Task<string?> GetReportCacheAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "reportCache");
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Clear report cache
    /// </summary>
    public async Task ClearReportCacheAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "reportCache");
    }
}
