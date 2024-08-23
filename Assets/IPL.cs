using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class IPLScheduler : MonoBehaviour
{
  public InputField team1Input;
  public InputField team2Input;
  public InputField venueInput;
  public InputField broadcastingTeamInput;
  public InputField securityTeamInput;
  public InputField priorityInput;
  public Text scheduledMatchesText;

  private List<Match> matches = new List<Match>();
  private int totalMatches = 74; // Adjust this value for the IPL season

  public void ResetData()
  {
    matches.Clear();
    scheduledMatchesText.text = "Scheduled Matches:\n"; // Clear the display as well
  }

  public void UploadData()
  {
    string team1 = team1Input.text;
    string team2 = team2Input.text;
    string venue = venueInput.text;
    string broadcastingTeam = broadcastingTeamInput.text;
    string securityTeam = securityTeamInput.text;

    // Validate user input
    if (string.IsNullOrEmpty(team1) || string.IsNullOrEmpty(team2) || string.IsNullOrEmpty(venue))
    {
      Debug.LogError("Please fill in all required fields (Team 1, Team 2, Venue)");
      return;
    }

    int priority;
    if (!int.TryParse(priorityInput.text, out priority))
    {
      Debug.LogError("Invalid priority. Please enter an integer value.");
      return;
    }

    Match match = new Match(team1, team2, venue, broadcastingTeam, securityTeam, priority);
    matches.Add(match);

    // Clear input fields after reading data
    team1Input.text = string.Empty; // Use string.Empty for guaranteed clearing
    team2Input.text = string.Empty;
    venueInput.text = string.Empty;
    broadcastingTeamInput.text = string.Empty;
    securityTeamInput.text = string.Empty;
    priorityInput.text = string.Empty;
  }

  public void ScheduleMatches()
  {
    if (matches.Count > totalMatches)
    {
      Debug.LogError("Number of uploaded matches exceeds the total IPL season matches.");
      return;
    }

    // Sort matches based on priority
    matches.Sort((x, y) => y.Priority.CompareTo(x.Priority));

    List<DateTime> scheduledDates = new List<DateTime>();
    foreach (Match match in matches)
    {
      DateTime scheduledDate = FindAvailableDate(scheduledDates, match);
      if (scheduledDate == DateTime.MinValue)
      {
        Debug.LogError("Failed to schedule match: " + match.Team1 + " vs " + match.Team2);
        continue;
      }
      match.Date = scheduledDate;
      scheduledDates.Add(scheduledDate);
    }

    // Display scheduled matches
    DisplayScheduledMatches();

    // Clear the matches list after scheduling
    matches.Clear();
  }

  private DateTime FindAvailableDate(List<DateTime> scheduledDates, Match match)
  {
    DateTime currentDate = DateTime.Now.AddDays(2); // Start two days ahead
    int lookaheadWindow = 30; // Look ahead for 30 days (adjustable)

    for (int i = 0; i < lookaheadWindow; i++)
    {
      int scheduledMatchesOnDate = 0;

      // Check if any match is scheduled for the current date
      foreach (DateTime date in scheduledDates)
      {
        if (date.Date == currentDate.Date)
        {
          scheduledMatchesOnDate++;
          if (scheduledMatchesOnDate > 1 && currentDate.DayOfWeek != DayOfWeek.Sunday)
          {
            break; // Stop checking if there is already one match on a non-Sunday date
          }
        }
      }

      if (scheduledMatchesOnDate < 1 || (currentDate.DayOfWeek == DayOfWeek.Sunday && scheduledMatchesOnDate < 2))
      {
        // Check if the current date meets the 2-day gap requirement
        if (IsGapValid(currentDate, match))
        {
          return currentDate; // Return the current date if no conflict and two-day gap is met
        }
      }

      currentDate = currentDate.AddDays(1); // Move to the next day within the lookahead window
    }

    // No suitable date found within the lookahead window (could indicate scheduling overload)
    Debug.LogError("No available date found for match: " + match.Team1 + " vs " + match.Team2 + ". Maximum one match allowed per day, two on Sundays.");
    return DateTime.MinValue;
  }

  private bool IsGapValid(DateTime currentDate, Match match)
  {
    // Find the last match dates for both teams involved
    DateTime lastTeam1MatchDate = DateTime.MinValue;
    DateTime lastTeam2MatchDate = DateTime.MinValue;
    DateTime lastBroadcastingMatchDate = DateTime.MinValue;
    DateTime lastSecurityMatchDate = DateTime.MinValue;

    foreach (Match scheduledMatch in matches)
    {
      if (scheduledMatch.Team1 == match.Team1 || scheduledMatch.Team2 == match.Team1)
      {
        lastTeam1MatchDate = scheduledMatch.Date;
      }
      if (scheduledMatch.Team1 == match.Team2 || scheduledMatch.Team2 == match.Team2)
      {
        lastTeam2MatchDate = scheduledMatch.Date;
      }
      if (scheduledMatch.BroadcastingTeam == match.BroadcastingTeam)
      {
        lastBroadcastingMatchDate = scheduledMatch.Date;
      }
      if (scheduledMatch.SecurityTeam == match.SecurityTeam)
      {
        lastSecurityMatchDate = scheduledMatch.Date;
      }
    }

    // Ensure a minimum two-day gap between matches involving the same teams or resources
    return (lastTeam1MatchDate == DateTime.MinValue || currentDate.Subtract(lastTeam1MatchDate).Days >= 2) &&
           (lastTeam2MatchDate == DateTime.MinValue || currentDate.Subtract(lastTeam2MatchDate).Days >= 2) &&
           (lastBroadcastingMatchDate == DateTime.MinValue || currentDate.Subtract(lastBroadcastingMatchDate).Days >= 2) &&
           (lastSecurityMatchDate == DateTime.MinValue || currentDate.Subtract(lastSecurityMatchDate).Days >= 2);
  }

  private void DisplayScheduledMatches()
  {
    scheduledMatchesText.text = "Scheduled Matches:\n";
    foreach (Match match in matches)
    {
      scheduledMatchesText.text += $"Priority: {match.Priority}, {match.Team1} vs {match.Team2} at {match.Venue}, Broadcasting Team: {match.BroadcastingTeam}, Security Team: {match.SecurityTeam}, {match.Date.ToString("dd-MMM-yyyy")}\n";
    }
  }
}

public class Match
{
  public string Team1 { get; }
  public string Team2 { get; }
  public string Venue { get; }
  public string BroadcastingTeam { get; }
  public string SecurityTeam { get; }
  public int Priority { get; }
  public DateTime Date { get; set; }

  public Match(string team1, string team2, string venue, string broadcastingTeam, string securityTeam, int priority)
  {
    Team1 = team1;
    Team2 = team2;
    Venue = venue;
    BroadcastingTeam = broadcastingTeam;
    SecurityTeam = securityTeam;
    Priority = priority;
  }
}
