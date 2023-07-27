using Mars.Interfaces.Agents;
using Mars.Interfaces.Environments;

namespace LaserTagBox.Model.Refugee;

public interface ISocialNetwork : IPositionable, IEntity
{
 /// <summary>
 /// Adds "other" to the set of friends and adds callee to the "other"'s set of friends.
 /// If they're already friends, the content of the friend sets will stay the same.
 ///
 /// </summary>
 /// <param name="other"> Another class that implements the ISocialNetwork interface and has a set of friends to update</param>
 
 public void UpdateSocialNetwork(ISocialNetwork other){}
}