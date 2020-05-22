declare namespace soundbyte {
  export namespace navigation {
    export function navigateTo(viewModel: string, args: any): boolean;
    export function navigateTo(viewModel: string): boolean;
  }

  export namespace settings {
    export function getPreference(key: string): string;
    export function getSecure(key: string): string;

    export function removePreference(key: string): void;
    export function removeSecure(key: string): void;

    export function setPreference(key: string, value: string): void;
    export function setSecure(key: string, value: string): void;
  }

  export namespace network {
    export function performRequest(url: string): string;
    export function performAnonymousRequest(url: string): string;
  }

  export namespace interop {
    export namespace youtube {
      export function getVideoStream(id: string): string;
      export function getAudioStream(id: string): string;
    }
  }

  export function timeFromMilliseconds(ms: number): any;

  export enum MediaType {
    Unknown,
    Track,
    User,
    Playlist,
    Podcast
  }

  export class Media {
    mediaType: MediaType;
    id: string;
    musicProviderId: string;
  }

  export class Track extends Media {
    trackId: string;
    link: string;
    isLive: boolean;
    audioStreamUrl: string;
    videoStreamUrl: string;
    artworkUrl: string;
    title: string;
    description: string;
    duration: number;
    created: Date;
    user: User;
  }

  export class User extends Media {
    userId: string;
    username: string;
    artworkUrl: string;
    description: string;
  }

  export class Playlist extends Media {
    playlistId: string;
    link: string;
    title: string;
    description: string;
    duration: number;
    created: Date;
    user: User;
    artworkUrl: string;
  }

  export class Podcast extends Media {}

  export class SourceResponse {
    constructor(items: Media[], nextToken: string);
    constructor(errorTitle: string, errorContent: string);
  }

  export class FilteredListViewModelHolder {
    constructor(model: any, title: string, filters: FilterViewItem[]);

    model: any;
    title: string;
    filters: FilterViewItem[];
  }

  export class FilterViewItem {
    constructor(isHeader: boolean, displayName: string);
    constructor(displayName: string);
    constructor(displayName: string, filterName: string);

    filterName: string;
    displayName: string;
  }

  export class GenericListViewModelHolder {
    constructor(model: any, title: string);

    model: any;
    title: string;
  }
}

// Allow other scripts to use this definition
export = soundbyte;
export as namespace soundbyte;
