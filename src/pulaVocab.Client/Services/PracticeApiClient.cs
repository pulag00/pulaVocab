using System.Net.Http.Json;
using pulaVocab.Application.Practice;
using pulaVocab.Domain.Enums;

namespace pulaVocab.Client.Services;

public interface IPracticeApiClient { Task<PracticeStatisticsResponse> Statistics(Language language, CancellationToken ct=default); Task<IReadOnlyList<PracticeWordResponse>> Preview(PracticeFilterRequest filter, CancellationToken ct=default); Task<PracticeSessionResponse> Start(StartPracticeRequest request, CancellationToken ct=default); Task<PracticeSessionResponse> Answer(Guid id, SubmitPracticeAnswerRequest request, CancellationToken ct=default); Task<PracticeSessionResponse> Finish(Guid id, bool early, CancellationToken ct=default); }
public sealed class PracticeApiClient(HttpClient http) : IPracticeApiClient
{
    public async Task<PracticeStatisticsResponse> Statistics(Language l,CancellationToken ct=default)=>await http.GetFromJsonAsync<PracticeStatisticsResponse>($"api/practice/statistics?language={l}",ct)??new();
    public async Task<IReadOnlyList<PracticeWordResponse>> Preview(PracticeFilterRequest f,CancellationToken ct=default)=>await Send<PracticeFilterRequest,List<PracticeWordResponse>>("api/practice/preview",f,ct);
    public Task<PracticeSessionResponse> Start(StartPracticeRequest r,CancellationToken ct=default)=>Send<StartPracticeRequest,PracticeSessionResponse>("api/practice/sessions",r,ct);
    public Task<PracticeSessionResponse> Answer(Guid id,SubmitPracticeAnswerRequest r,CancellationToken ct=default)=>Send<SubmitPracticeAnswerRequest,PracticeSessionResponse>($"api/practice/sessions/{id}/answers",r,ct);
    public Task<PracticeSessionResponse> Finish(Guid id,bool early,CancellationToken ct=default)=>Send<FinishPracticeRequest,PracticeSessionResponse>($"api/practice/sessions/{id}/finish",new(){EndedEarly=early},ct);
    private async Task<TOut> Send<TIn,TOut>(string url,TIn body,CancellationToken ct){var response=await http.PostAsJsonAsync(url,body,ct);response.EnsureSuccessStatusCode();return (await response.Content.ReadFromJsonAsync<TOut>(cancellationToken:ct))!;}
}
